from tqdm import tqdm
import urllib.request
import os
import urllib.request
import zipfile
import random
from fuzzywuzzy import fuzz

class Menu:
    def __init__(self):
        self.options = {
            "1": self.read_data,
            "2": self.apply_entropy,
            "3": self.string_match,
            "4": self.exit_program
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
        # Implement the logic for the "Apply Entropy" option here
        print("Apply Entropy option selected")

    def string_match(self):
        # Implement the logic for the "String Match" option here
        print("String Match option selected")
        try:
            address_dicts =  load_into_memory('./data/au.csv')
            #print the first 10 addresses without fields
            for address_dict in address_dicts[:10]:
                print(address_dict)
                
            # fuzzy string match with the first 10 addresses
            street_name = input("Enter a street name: ").upper()

            #clear the screen
            os.system('cls' if os.name == 'nt' else 'clear')

            # print the name of the street with the highest match
            print("Is your street", max(address_dicts, key=lambda x: fuzz.ratio(street_name, x['street']))['street'], "?")
            
            
        except FileNotFoundError:
            self.read_data()
        
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
    open_ratio = 0.1
    
    fields = ['id', 'number', 'street', 'unit', 'lon',
              'lat', 'city', 'postcode', 'region', 'accuracy']
    addresses = []
    
    with open(filename, 'r') as file:
        for line in file:
            if random.random() < open_ratio:
                addresses.append(line)

            if len(addresses) > 999:
                    break
                
    address_dicts = [dict(zip(fields, [value.strip() for value in address.split(',')])) for address in addresses]
    
    
      
    return address_dicts