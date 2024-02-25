import csv
from tqdm import tqdm
import urllib.request
import os
import urllib.request
import zipfile
import random
import entropy
from fuzzywuzzy import fuzz
import address_class


class Menu:

    def __init__(self):
        self.options = {
            "1": self.read_data,
            "2": self.apply_entropy,
            "3": self.string_match,
            "4": self.exit_program,
            "5": self.get_street_types
        }

    def display_menu(self):
        print("Menu:")
        print("1. Read Data")
        print("2. Apply Entropy")
        print("3. String Match")
        print("4. Exit")

    def read_data(self):
        # Implement the logic for the "Read Data" option here

        zip_file = './data/au-nov2023.zip'
        url = "https://f001.backblazeb2.com/file/alantgeo-public/au-nov2023.zip"
        data_file = './data/au.csv'

        print("Read Data option selected")
        try:
            open(data_file, 'r')
        except FileNotFoundError:
            print("File not found. Would you like to download it?")
            print("y/n")
            choice = input()
            if choice.lower() == 'y':

                download_file(url, zip_file)

                unzip_file(zip_file)
                print("File downloaded and unzipped.")

    def apply_entropy(self):

        try:
            misspelt_addresses = load_into_memory('./data/au.csv')
            print("Apply Entropy option selected")
            print("Applying entropy to the addresses...")
            for address in misspelt_addresses:
                address['street'] = entropy.simulate_typing_errors(
                    address['street'])

            # save as csv
            with open('./data/au_misspelt.csv', 'w', newline='') as file:
                writer = csv.writer(file)
                writer.writerow(["id", "number", "street", "unit", "lon",
                                "lat", "city", "postcode", "region", "accuracy"])
                for address in misspelt_addresses:
                    writer.writerow(address.values())
        except FileNotFoundError:
            self.read_data()

    def string_match(self):
        # Implement the logic for the "String Match" option here
        print("String Match option selected")
        try:
            misspelt_address_dict = load_into_memory('./data/au_misspelt.csv')
            correct_address_dict = load_into_memory('./data/au.csv')

            misspelt_streets = [address['street']
                                for address in misspelt_address_dict]

            correct_streets = [address['street']
                               for address in correct_address_dict]

            entropy.string_match(misspelt_streets, correct_streets)

        except FileNotFoundError:
            self.apply_entropy()

    def get_street_types(self, file='./data/au.csv'):
        load_into_memory(file)
        street_types = []

        with open(file, 'r') as file:
            for line in file:

                street_types.append(line.split(',')[2].split(' ')[-1])

        unique_street_types = set(street_types)
        print(unique_street_types)

        return unique_street_types

    def exit_program(self):
        print("Exiting the program...")
        exit()

    def run(self):
        while True:
            self.display_menu()
            choice = input("Enter your choice: ")
            if choice in self.options:
                self.options[choice]()
            else:
                print("Invalid choice. Please try again.")


def unzip_file(filename):
    print("Unzipping file...")
    with zipfile.ZipFile(filename, 'r') as zip_ref:
        zip_ref.extractall('./data')

    # delete the zip file
    os.remove(filename)


def download_file(url, filename):

    url = "https://f001.backblazeb2.com/file/alantgeo-public/au-nov2023.zip"
    downloaded_filename = filename

    print("Downloading file...")

    if not os.path.exists('./data'):
        os.makedirs('./data')

    # Download the file from `url` and save it locally under `file_name`:
    # Use tqdm to show download progress
    with tqdm(unit='B', unit_scale=True, unit_divisor=1024, miniters=1, desc=downloaded_filename) as t:
        def reporthook(blocknum, blocksize, totalsize):
            t.total = totalsize
            t.update(blocknum * blocksize - t.n)

        urllib.request.urlretrieve(
            url, downloaded_filename, reporthook=reporthook)

    return downloaded_filename


def load_into_memory(filename):

    addresses = []
    with open(filename, 'r') as file:
        reader = csv.reader(file, delimiter=',')
        # Skip the header
        next(reader, None)
        for row in reader:
            address = address_class.Address(
                row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9])
            addresses.append(address.__dict__)

            if len(addresses) > 1000:
                break

    return addresses
