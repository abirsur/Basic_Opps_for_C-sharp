# Function to process data
def process_data(data):
    df = pd.DataFrame(data)
    df['usagedate'] = pd.to_datetime(df['usagedate'])
    df['Week'] = df['usagedate'].dt.to_period('W').apply(lambda r: r.start_time)
    type_cost = df.groupby(['resourcetype', 'Week'])['cost'].sum().reset_index()
    pivot_table = type_cost.pivot(index='resourcetype', columns='Week', values='cost').fillna(0)
    pivot_table.reset_index(inplace=True)
    pivot_table.insert(0, 'SlNo', range(1, len(pivot_table) + 1))
    return pivot_table

# Function to create Excel report
def create_excel_report(pivot_table, output_file):
    with pd.ExcelWriter(output_file, engine='xlsxwriter') as writer:
        pivot_table.to_excel(writer, sheet_name='Type Cost', index=False)
        
        workbook = writer.book
        worksheet = writer.sheets['Type Cost']
        
        # Add a chart
        chart = workbook.add_chart({'type': 'column'})
        for i in range(1, len(pivot_table.columns)):
            chart.add_series({
                'name':       ['Type Cost', 0, i],
                'categories': ['Type Cost', 1, 0, len(pivot_table), 0],
                'values':     ['Type Cost', 1, i, len(pivot_table), i],
            })
        worksheet.insert_chart('E2', chart)
