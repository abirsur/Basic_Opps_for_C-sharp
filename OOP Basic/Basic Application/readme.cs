import json
import pandas as pd
from azure.storage.blob import BlobServiceClient, BlobClient, ContainerClient

# JSON data
data = {
  "Client_Name": {"value": "ABC Pvt Ltd", "source": "Acord 125"},
  "Client_Billing_Street": {"value": "123 Main St", "source": "Email"},
  "Client_Billing_City": {"value": "New York", "source": "Acord 125"},
  "Client_Billing_Zip": {"value": "10001", "source": "LLM Derived"},
  "Client_Billing_State": {"value": "NY", "source": "Acord 125"},
  "Client_Billing_Country": {"value": "USA", "source": "System Generated"},
  "Client_Email_Address": {"value": "client@example.com", "source": "Email"},
  "Client_SIC": {"value": "7311", "source": "Acord 125"},
  "Client_NAICS": {"value": "541611", "source": "System Generated"},
  "Client_FEIN": {"value": "12-3456789", "source": "Email"},
  "Policy_Line_of_Business": {"value": "General Liability", "source": "Acord 125"},
  "Policy_Effective_Date": {"value": "2024-01-01", "source": "Acord 125"},
  "Policy_Expiration_Date": {"value": "2025-01-01", "source": "Acord 125"},
  "Broker_Agency_Name": {"value": "XYZ Insurance Brokers", "source": "Broker"},
  "Broker_Producer_Contact_Name": {"value": "John Doe", "source": "Broker"},
  "Broker_Producer_Street": {"value": "456 Market St", "source": "Broker"},
  "Broker_Producer_City": {"value": "San Francisco", "source": "Broker"},
  "Broker_Producer_Zip": {"value": "94105", "source": "Broker"},
  "Broker_Producer_State": {"value": "CA", "source": "Broker"},
  "Broker_Producer_Country": {"value": "USA", "source": "LLM Derived"},
  "Broker_Phone_Number": {"value": "555-123-4567", "source": "Broker"},
  "Broker_Producer_Email_Address": {"value": "broker@example.com", "source": "Email"}
}

# Create a list of dictionaries for DataFrame
rows = []
for field_name, details in data.items():
    row = {
        "Fields_Name": field_name,
        "Value": details["value"],
        "Source": details["source"]
    }
    rows.append(row)

# Convert list of dictionaries to DataFrame
df = pd.DataFrame(rows)

# Write DataFrame to Excel file
excel_file = "output.xlsx"
df.to_excel(excel_file, index=False)

# Azure Blob Storage configuration
connection_string = "your_connection_string"
container_name = "your_container_name"
blob_name = "output.xlsx"

# Upload the Excel file to Azure Blob Storage
blob_service_client = BlobServiceClient.from_connection_string(connection_string)
blob_client = blob_service_client.get_blob_client(container=container_name, blob=blob_name)

with open(excel_file, "rb") as data:
    blob_client.upload_blob(data, overwrite=True)

# Generate the blob file link
blob_url = f"https://{blob_service_client.account_name}.blob.core.windows.net/{container_name}/{blob_name}"

# Generate new JSON structure with empty values and add the blob file link
new_data = {
    "Client_Name": "ABC Pvt Ltd", 
    "Client_Billing_Street": "",
    "Client_Billing_City": "",
    "Client_Billing_Zip": "",
    "Client_Billing_State": "",
    "Client_Billing_Country": "",
    "Client_Email_Address": "",
    "Client_SIC": "",
    "Client_NAICS": "",
    "Client_FEIN": "",
    "Policy_Line_of_Business": "",
    "Policy_Effective_Date": "",
    "Policy_Expiration_Date": "",
    "Broker_Agency_Name": "",
    "Broker_Producer_Contact_Name": "",    
    "Broker_Producer_Street": "",
    "Broker_Producer_City": "",
    "Broker_Producer_Zip": "",
    "Broker_Producer_State": "",
    "Broker_Producer_Country": "",
    "Broker_Phone_Number": "",
    "Broker_Producer_Email_Address": "",
    "ResponseFile": blob_url
}

# Save the new JSON structure to a file
with open("new_data.json", "w") as json_file:
    json.dump(new_data, json_file, indent=4)
