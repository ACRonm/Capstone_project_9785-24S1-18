from tqdm import tqdm


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

        def unzip_file(filename):
            import zipfile
            with zipfile.ZipFile(filename, 'r') as zip_ref:
                zip_ref.extractall('./data')

        print("Read Data option selected")
        try:
            open('./data/au.csv', 'r')
        except FileNotFoundError:
            print("File not found. Would you like to download it?")
            print("y/n")
            choice = input()
            if choice.lower() == 'y':
                # download the file
                print("Downloading file...")
                try:
                    unzip_file('./data/au-nov2023.zip')
                except FileNotFoundError:
                    print("File downloaded and unzipped.")

                    import urllib.request
                    url = "https://f001.backblazeb2.com/file/alantgeo-public/au-nov2023.zip"
                    downloaded_filename = "./data/au-nov2023.zip"

                    # Download the file from `url` and save it locally under `file_name`:
                    # Use tqdm to show download progress
                    with tqdm(unit='B', unit_scale=True, unit_divisor=1024, miniters=1, desc=downloaded_filename) as t:

                        def reporthook(blocknum, blocksize, totalsize):
                            t.total = totalsize
                            t.update(blocknum * blocksize - t.n)

                        urllib.request.urlretrieve(
                            url, downloaded_filename, reporthook=reporthook)

                    unzip_file(downloaded_filename)

    def apply_entropy(self):
        # Implement the logic for the "Apply Entropy" option here
        print("Apply Entropy option selected")

    def string_match(self):
        # Implement the logic for the "String Match" option here
        print("String Match option selected")

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
