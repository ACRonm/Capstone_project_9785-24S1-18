import csv
import pandas as pd

input_file = "./data/au.csv"

output_file = "./data/au_deduplicated.csv"

data = pd.read_csv(input_file, header=0, encoding="utf-8")

# remove all duplicates if they have the same "street" and "postcode" and "city"

data = data.drop_duplicates(
    subset=["street", "postcode", "city"], keep="first")

data = data[data["postcode"].notna()]
data["postcode"] = data["postcode"].astype(int)

data.to_csv(output_file, index=False)

print("Deduplicated data is saved to", output_file)
