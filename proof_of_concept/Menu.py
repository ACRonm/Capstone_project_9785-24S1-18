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
        print("Read Data option selected")

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


# Create an instance of the Menu class and run the program
menu = Menu()
menu.run()
