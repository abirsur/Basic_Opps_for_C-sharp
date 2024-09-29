import requests
import pandas as pd
import xlsxwriter
from datetime import datetime, timedelta

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

# Function to process data
def process_data(data):
    df = pd.json_normalize(data['properties']['rows'])
    df.columns = ['Date', 'ResourceId', 'ResourceType', 'Cost']
    df['Date'] = pd.to_datetime(df['Date'])
    df['Week'] = df['Date'].dt.to_period('W').apply(lambda r: r.start_time)
    resource_cost = df.groupby(['ResourceId', 'Week'])['Cost'].sum().reset_index()
    type_cost = df.groupby(['ResourceType', 'Week'])['Cost'].sum().reset_index()
    return resource_cost, type_cost

# Function to create Excel report
def create_excel_report(resource_cost, type_cost, output_file):
    with pd.ExcelWriter(output_file, engine='xlsxwriter') as writer:
        resource_cost.to_excel(writer, sheet_name='Resource Cost', index=False)
        type_cost.to_excel(writer, sheet_name='Type Cost', index=False)
        
        workbook = writer.book
        worksheet = writer.sheets['Resource Cost']
        
        chart = workbook.add_chart({'type': 'column'})
        chart.add_series({
            'categories': ['Resource Cost', 1, 1, len(resource_cost), 1],
            'values': ['Resource Cost', 1, 2, len(resource_cost), 2],
            'name': 'Resource Cost'
        })
        worksheet.insert_chart('E2', chart)

        worksheet = writer.sheets['Type Cost']
        chart = workbook.add_chart({'type': 'column'})
        chart.add_series({
            'categories': ['Type Cost', 1, 1, len(type_cost), 1],
            'values': ['Type Cost', 1, 2, len(type_cost), 2],
            'name': 'Type Cost'
        })
        worksheet.insert_chart('E2', chart)

# Main function
def main(subscription_id, start_date, end_date, access_token, output_file):
    data = fetch_cost_data(subscription_id, start_date, end_date, access_token)
    resource_cost, type_cost = process_data(data)
    create_excel_report(resource_cost, type_cost, output_file)

# Example usage
subscription_id = 'your_subscription_id'
start_date = '2023-01-01'
end_date = '2023-01-31'
access_token = 'your_access_token'
output_file = 'azure_cost_report.xlsx'

main(subscription_id, start_date, end_date, access_token, output_file)
