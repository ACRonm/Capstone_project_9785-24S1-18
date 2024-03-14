import geopandas as gpd
import pandas as pd
from shapely.geometry import Point
import json

data = gpd.read_file('./data/source.geojson')

# Convert to GeoDataFrame
gdf = gpd.GeoDataFrame(data, geometry='geometry')


# Convert to CSV
gdf.to_csv('act.csv', index=False)
