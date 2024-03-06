import random
import string
from tqdm import tqdm


def simulate_errors(address_string):

    # Define operation functions
    def random_insertion(address_string):
        rand_index = random.randint(0, len(address_string))
        rand_char = random.choice(string.ascii_letters)
        return address_string[:rand_index] + rand_char + address_string[rand_index:]

    def random_deletion(address_string):
        if len(address_string) == 0:
            return address_string
        rand_index = random.randint(0, len(address_string) - 1)
        return address_string[:rand_index] + address_string[rand_index + 1:]

    def random_substitution(address_string):
        if len(address_string) == 0:
            return address_string
        rand_index = random.randint(0, len(address_string) - 1)
        rand_char = random.choice(string.ascii_letters)
        return address_string[:rand_index] + rand_char + address_string[rand_index + 1:]

    def random_transposition(address_string):
        if len(address_string) < 2:
            return address_string
        rand_index = random.randint(0, len(address_string) - 2)
        return address_string[:rand_index] + address_string[rand_index + 1] + address_string[rand_index] + address_string[rand_index + 2:]

    def ocr_error(address_string):
        if len(address_string) == 0:
            return address_string

        # ocr errors are a common source of errors in address data
        # we will simulate this by randomly replacing a character with a similar looking character
        # for example, 0 with O, 1 with I, 2 with Z, etc.
        # we will also add some random noise to the address

        # Define a dictionary of similar looking characters
        similar_chars = {
            "O": "0",
            "0": "O",
            "I": "1",
            "1": "I",
            "Z": "2",
            "2": "Z",
            "S": "5",
            "5": "S",
        }

        # if the string contains a number in the similar_chars dictionary, replace it with a similar looking letter

        ocr_error_rate = 0.1

        for i in range(len(address_string)):

            if address_string[i] in similar_chars:
                if random.random() < ocr_error_rate:
                    replacement = random.choice(
                        similar_chars[address_string[i]])

                    address_string = address_string[:i] + \
                        replacement + address_string[i + 1:]

        return address_string

    # Choose a random operation to apply
    operations = [random_insertion, random_deletion,
                  random_substitution, random_transposition, random_transposition, ocr_error, ocr_error, ocr_error]

    rand_operation = random.choice(operations)

    return rand_operation(address_string)
