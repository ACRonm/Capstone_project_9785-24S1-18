import firebase_admin
from firebase_admin import ml
from firebase_admin import credentials
import keras

import tensorflow as tf

cred = credentials.Certificate(
    r'AddressStandardisation\streetstandardisation-firebase-adminsdk-d65t2-de0cb0478f.json')

firebase_admin.initialize_app(cred,
                              options={'storageBucket': 'gs://streetstandardisation.appspot.com'})


model = tf.keras.models.load_model('ML/street_type_model.h5')

source = ml.TFLiteGCSModelSource.from_keras_model(model)

tflite_format = ml.TFLiteFormat(model_source=source)

model = ml.Model(
    display_name='street_standardisation_model',
    tags=['address', 'standardisation'],
    model_format=tflite_format
)

new_model = ml.create_model(model)
# The first time you publish a model, it will become the default version.
ml.publish_model(new_model.model_id)
