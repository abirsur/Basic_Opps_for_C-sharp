# Function to process data
def process_data(data, group_by):
    df = pd.DataFrame(data)
    df['usagedate'] = pd.to_datetime(df['usagedate'])
    df['Week'] = df['usagedate'].dt.to_period('W').apply(lambda r: r.start_time)
    weekly_cost = df.groupby([group_by, 'Week'])['cost'].sum().unstack(fill_value=0).reset_index()
    return weekly_cost

# Function to create Excel report
def create_excel_report(resource_type_cost, resource_id_cost, output_file):
    with pd.ExcelWriter(output_file, engine='xlsxwriter') as writer:
        # Write ResourceType sheet
        resource_type_cost.to_excel(writer, sheet_name='Resource Type Cost', index=False, startrow=1)
        workbook = writer.book
        worksheet = writer.sheets['Resource Type Cost']
        
        # Add headers for ResourceType sheet
        worksheet.write(0, 0, 'SlNo')
        worksheet.write(0, 1, 'ResourceType')
        for col_num, week in enumerate(resource_type_cost.columns[1:], start=2):
            worksheet.write(0, col_num, f"Week ({week.strftime('%m-%d')} to {(week + timedelta(days=6)).strftime('%m-%d')})")
        
        # Add SlNo column for ResourceType sheet
        for row_num in range(1, len(resource_type_cost) + 1):
            worksheet.write(row_num, 0, row_num)
        
        # Write ResourceId sheet
        resource_id_cost.to_excel(writer, sheet_name='Resource ID Cost', index=False, startrow=1)
        worksheet = writer.sheets['Resource ID Cost']
        
        # Add headers for ResourceId sheet
        worksheet.write(0, 0, 'SlNo')
        worksheet.write(0, 1, 'ResourceId')
        for col_num, week in enumerate(resource_id_cost.columns[1:], start=2):
            worksheet.write(0, col_num, f"Week ({week.strftime('%m-%d')} to {(week + timedelta(days=6)).strftime('%m-%d')})")
        
        # Add SlNo column for ResourceId sheet
        for row_num in range(1, len(resource_id_cost) + 1):
            worksheet.write(row_num, 0, row_num)

# Main function
def main(subscription_id, start_date, end_date, json_file, output_file):
    access_token = get_access_token()
    data = fetch_cost_data(subscription_id, start_date, end_date, access_token)
    transformed_data = transform_data(data)
    save_data_to_json(transformed_data, json_file)
    
    data = load_data_from_json(json_file)
    resource_type_cost = process_data(data, 'resourcetype')
    resource_id_cost = process_data(data, 'resourceid')
    create_excel_report(resource_type_cost, resource_id_cost, output_file)
