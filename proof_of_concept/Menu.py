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
import spell_correct


class Menu:

    def __init__(self):
        self.options = {
            "1": self.read_data,
            "2": self.apply_entropy,
            "3": self.string_match,
            "4": self.exit_program,
            # "5": self.get_street_types
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

            for address in tqdm(misspelt_addresses, desc="Applying entropy"):
                address.street = entropy.simulate_errors(address.street)
                address.city = entropy.simulate_errors(address.city)

            # save as csv
            write_to_csv('./data/au_misspelt.csv', misspelt_addresses)
            print("Entropy applied to the addresses and saved to file.")
        except FileNotFoundError:
            self.read_data()

    def string_match(self):
        repaired_addresses = []
        wrong_addresses = []
        # Implement the logic for the "String Match" option here
        print("String Match option selected")

        # load the misspelt addresses
        misspelt_addresses = load_into_memory('./data/au_misspelt.csv')
        # load the correct addresses
        correct_addresses = load_into_memory('./data/au.csv', read_all=True)

        repaired_addresses, wrong_addresses = spell_correct.correct_spelling(
            misspelt_addresses, correct_addresses)

        print(f"Repaired addresses: {len(repaired_addresses)}")
        print(f"Wrong addresses: {len(wrong_addresses)}")

        # save as csv
        write_to_csv('./data/au_repaired.csv', repaired_addresses)
        # save wrong addresses as csv
        write_to_csv('./data/au_wrong.csv', wrong_addresses)

        repaired_addresses = load_into_memory('./data/au_repaired.csv')

        spell_correct.check_results(
            repaired_addresses, correct_addresses)

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


def load_into_memory(filename, read_all=False):

    addresses = []
    with open(filename, 'r') as file:
        reader = csv.reader(file, delimiter=',')
        # Skip the header
        next(reader, None)

        for row in reader:
            # create a new address object
            address = address_class.Address(
                row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9])
            addresses.append(address)

            if read_all:
                # load all addresses
                continue
            else:
                if len(addresses) > 10000:
                    break
    return addresses


def write_to_csv(filename, data):
    with open(filename, 'w', newline='') as file:
        writer = csv.writer(file)
        writer.writerow(["id", "number", "street", "unit", "lon",
                        "lat", "city", "postcode", "region", "accuracy"])
        for address in data:
            writer.writerow(address.to_list())
