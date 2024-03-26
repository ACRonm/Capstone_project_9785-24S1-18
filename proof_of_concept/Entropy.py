import random
import string
from tqdm import tqdm


def simulate_errors(address_string):

    address_string = simulate_street_type_abbreviation(address_string)

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
                  random_substitution, random_transposition, random_transposition, ocr_error]

    rand_operation = random.choice(operations)

    return rand_operation(address_string)


def simulate_street_type_abbreviation(address_string):

    # Define a dictionary of common street type abbreviations
    street_type_abbreviations = {
        "Avenue": {"Ave", "Ave.", "Av"},
        "Boulevard": ["Blvd", "Blvd.", "Bv"],
        "Promenade": ["Prom", "Prom."],
        "Drive": {"Dr", "Dr.", "Drv"},
        "Street": {"St", "St."},
        "Road": {"Rd", "Rd.", "R"},
        "Lane": {"Ln", "Ln.", "La"},
        "Parkway": {"Pkwy", "Pkwy.", "Pky"},
        "Circle": {"Cir", "Cir.", "C"},
        "Court": {"Ct", "Ct."},
        "Square": {"Sq", "Sq."},
        "Highway": {"Hwy", "Hwy."},
        "Plaza": {"Plz", "Plz."},
        "Terrace": {"Ter", "Ter."},
        "Ridge": {"Rdg", "Rdg."},
        "Crossing": "Xing",
        "Grove": {"Grv", "Grv.", "Gr"},
        "Point": {"Pt", "Pt."},
        "Creek": {"Crk", "Crk."},
        "Ridge": {"Rdg", "Rdg."},
        "View": {"Vw", "Vw."},
        "Crescent": {"Cres", "Cres.", "Cr.", "Cr"},
        "Cove": {"Cv", "Cv."},
        "Gardens": {"Gdns", "Gdns."},
        "Green": {"Grn", "Grn."},
        "Terrace": {"Ter", "Ter."},
        "Place": {"Pl", "Pl."},
        "Circuit": {"Cct", "Cct."},
        "Parade": {"Pde", "Pde."},
        "Esplanade": {"Esp", "Esp."},
        "Mews": {"Mws", "Mws."},
        "Quay": {"Qy", "Qy."},
        "Walk": {"Wlk", "Wlk."},
        "Way": {"Wy", "Wy."},
        "Loop": {"Lp", "Lp."},
        "Track": {"Trk", "Trk."},
        "Link": {"Lnk", "Lnk."},
        "Vista": {"Vis", "Vis."},
        "Glade": {"Gld", "Gld."}
    }

    street_type_abbreviations = {
        k.upper(): [v.upper() for v in vs] for k, vs in street_type_abbreviations.items()}

    # separate the street type from the rest of the address
    address_parts = address_string.split(" ")
    street_type = address_parts[-1].strip()
    street_name = " ".join(address_parts[:-1])

    # if the street type is in the dictionary, replace it with a random abbreviation, with a 60% chance
    random_rate = 0.6

    if street_type in street_type_abbreviations:
        abbreviations = street_type_abbreviations[street_type]
        if random.random() < random_rate:
            street_type = random.choice(list(abbreviations))

            # print the list of street type abbreviations

            address_string = street_name + " " + street_type

    return address_string


def simulate_postcode_errors(postcode):
    # Define operation functions
    def random_insertion(postcode):
        rand_index = random.randint(0, len(postcode))
        rand_char = random.choice(string.digits)
        return postcode[:rand_index] + rand_char + postcode[rand_index:]

    def random_deletion(postcode):
        if len(postcode) == 0:
            return postcode
        rand_index = random.randint(0, len(postcode) - 1)
        return postcode[:rand_index] + postcode[rand_index + 1:]

    def random_substitution(postcode):
        if len(postcode) == 0:
            return postcode
        rand_index = random.randint(0, len(postcode) - 1)
        rand_char = random.choice(string.digits)
        return postcode[:rand_index] + rand_char + postcode[rand_index + 1:]

    def random_transposition(postcode):
        if len(postcode) < 2:
            return postcode
        rand_index = random.randint(0, len(postcode) - 2)
        return postcode[:rand_index] + postcode[rand_index + 1] + postcode[rand_index] + postcode[rand_index + 2:]

    def do_nothing(postcode):
        return postcode

        # ocr errors are a common source of errors in address data
        # we will simulate this by randomly replacing a character with a similar looking character
        # for example, 0 with O, 1 with I, 2 with Z, etc.
        # we will also add some random noise to the address

        # Define a dictionary of similar looking characters

        # if the string contains a number in the similar_chars dictionary, replace it with a similar looking letter

    # Choose a random operation to apply
    operations = [random_insertion, random_deletion,
                  random_substitution, random_transposition]

    if random.random() < 0.1:
        rand_operation = random.choice(operations)
    else:
        rand_operation = do_nothing

    return rand_operation(postcode)
