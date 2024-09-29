import pandas as pd
import xlsxwriter
from datetime import datetime, timedelta
import subprocess
import json
import requests

# Function to fetch access token using Azure CLI
def get_access_token():
    result = subprocess.run(['az', 'account', 'get-access-token', '--resource', 'https://management.azure.com/'], stdout=subprocess.PIPE)
    token = json.loads(result.stdout)
    return token['accessToken']

# Function to fetch cost data from Azure API
def fetch_cost_data(subscription_id, start_date, end_date, access_token):
    url = f"https://management.azure.com/subscriptions/{subscription_id}/providers/Microsoft.CostManagement/query?api-version=2019-11-01"
    headers = {
        'Authorization': f'Bearer {access_token}',
        'Content-Type': 'application/json'
    }
    body = {
        "type": "Usage",
        "timeframe": "Custom",
        "timePeriod": {
            "from": start_date,
            "to": end_date
        },
        "dataset": {
            "granularity": "Daily",
            "aggregation": {
                "totalCost": {
                    "name": "PreTaxCost",
                    "function": "Sum"
                }
            },
            "grouping": [
                {
                    "type": "Dimension",
                    "name": "ResourceId"
                },
                {
                    "type": "Dimension",
                    "name": "ResourceType"
                }
            ]
        }
    }
    response = requests.post(url, headers=headers, json=body)
    response.raise_for_status()
    return response.json()

# Function to transform data to the desired format
def transform_data(data):
    transformed_data = []
    for row in data['properties']['rows']:
        transformed_data.append({
            'usagedate': row[1],
            'resourcetype': row[3] if len(row) > 3 else None,
            'cost': row[0],
            'currency': row[4] if len(row) > 4 else None,
            'resourceid': row[2] if len(row) > 2 else None
        })
    return transformed_data

# Function to save data to JSON file
def save_data_to_json(data, file_path):
    with open(file_path, 'w') as f:
        json.dump(data, f, indent=4)

# Function to load data from JSON file
def load_data_from_json(file_path):
    with open(file_path, 'r') as f:
        return json.load(f)

# Function to process data
def process_data(data):
    df = pd.DataFrame(data)
    df['usagedate'] = pd.to_datetime(df['usagedate'])
    df['Week'] = df['usagedate'].dt.to_period('W').apply(lambda r: r.start_time)
    df['cost'] = df['cost'].round(2)
    return df

# Function to create Excel report with pivot table and chart
def create_excel_report(df, output_file):
    with pd.ExcelWriter(output_file, engine='xlsxwriter') as writer:
        df.to_excel(writer, sheet_name='Raw Data', index=False)
        
        workbook = writer.book
        worksheet = writer.sheets['Raw Data']
        
        # Create a pivot table
        pivot_table = pd.pivot_table(df, values='cost', index=['resourcetype'], columns=['Week'], aggfunc='sum', fill_value=0)
        pivot_table = pivot_table.reset_index()
        pivot_table.index.name = 'SlNo'
        pivot_table.columns.name = None
        
        # Add a total row
        pivot_table.loc['Total'] = pivot_table.sum(numeric_only=True)
        pivot_table.at['Total', 'resourcetype'] = 'Total'
        
        # Write the pivot table to a new sheet
        pivot_table.to_excel(writer, sheet_name='Pivot Table', startrow=1, index=True)
        
        worksheet = writer.sheets['Pivot Table']
        
        # Add headers
        worksheet.write(0, 0, 'SlNo')
        worksheet.write(0, 1, 'ResourceType')
        
        # Format the cost columns as currency
        currency_format = workbook.add_format({'num_format': 'INR 0.00'})
        for col_num in range(2, len(pivot_table.columns) + 1):
            worksheet.set_column(col_num, col_num, None, currency_format)
        
        # Create a scatter chart with lines and markers
        chart = workbook.add_chart({'type': 'scatter', 'subtype': 'straight_with_markers'})
        
        for i in range(1, len(pivot_table)):
            chart.add_series({
                'name':       ['Pivot Table', i + 1, 1],
                'categories': ['Pivot Table', 1, 2, 1, len(pivot_table.columns) - 1],
                'values':     ['Pivot Table', i + 1, 2, i + 1, len(pivot_table.columns) - 1],
            })
        
        chart.set_x_axis({'name': 'Week'})
        chart.set_y_axis({'name': 'Cost'})
        chart.set_title({'name': 'Resource Cost by Week'})
        
        worksheet.insert_chart('E2', chart)

# Main function
def main(subscription_id, start_date, end_date, json_file, output_file):
    access_token = get_access_token()
    data = fetch_cost_data(subscription_id, start_date, end_date, access_token)
    transformed_data = transform_data(data)
    save_data_to_json(transformed_data, json_file)
    
    data = load_data_from_json(json_file)
    df = process_data(data)
    create_excel_report(df, output_file)

# Example usage
subscription_id = 'your_subscription_id'
start_date = '2023-01-01'
end_date = '2023-01-31'
json_file = 'azure_cost_data.json'
output_file = 'azure_cost_report.xlsx'

main(subscription_id, start_date, end_date, json_file, output_file)
