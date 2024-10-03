import json
import pandas as pd

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

rows = []
for field_name, details in data.items():
    row = {
        "Fields_Name": field_name,
        "Value": details["value"],
        "Source": details["source"]
    }
    rows.append(row)
df = pd.DataFrame(rows)
df.to_excel("output.xlsx", index=False)
