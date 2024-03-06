import pandas as pd

# Load the CSV file
df = pd.read_csv('./data/au.csv')

# Convert the 'postcode' column to integer
df['postcode'] = df['postcode'].fillna(0).astype(int)

# Save the dataframe back to CSV
df.to_csv('./data/au.csv', index=False)
