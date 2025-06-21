import csv, sqlite3, os

STM_DIR = 'gtfs_stm'
con = sqlite3.connect("../stmDB.sqlite") # change to 'sqlite:///your_filename.db'

cur = con.cursor()

# Routes
cur.execute("CREATE TABLE routes (route_id integer PRIMARY KEY,agency_id,route_short_name,route_long_name,route_type,route_url,route_color,route_text_color);") # use your column names here
file_path = os.path.join(STM_DIR, 'routes.txt')
with open(file_path,'r') as fin:
    # csv.DictReader uses first line in file for column headings by default
    dr = csv.DictReader(fin) # comma is default delimiter    
    """
    for i in dr:
        print(i)
        print(i['route_id'])
        print(int(i['route_id']))
    """
    routes_db = [(int(i['route_id']),i['agency_id'],i['route_short_name'],i['route_long_name'],i['route_type'],i['route_url'],i['route_color'],i['route_text_color']) for i in dr]
    
cur.executemany("INSERT INTO routes (route_id,agency_id,route_short_name,route_long_name,route_type,route_url,route_color,route_text_color) VALUES (?, ?, ?, ?, ?, ?, ?, ?);", routes_db)

# Stops
cur.execute("CREATE TABLE stops (stop_id PRIMARY KEY,stop_code,stop_name,stop_lat,stop_lon,stop_url,location_type,parent_station,wheelchair_boarding);")
file_path = os.path.join(STM_DIR, 'stops.txt')
with open(file_path,'r') as fin:
    dr = csv.DictReader(fin) # comma is default delimiter    
    #for i in dr:
        #print(i)
    stops_db = [(i['stop_id'],i['stop_code'],i['stop_name'],i['stop_lat'],i['stop_lon'],i['stop_url'],i['location_type'],i['parent_station'],i['wheelchair_boarding']) for i in dr]
    
cur.executemany("INSERT INTO stops (stop_id,stop_code,stop_name,stop_lat,stop_lon,stop_url,location_type,parent_station,wheelchair_boarding) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);", stops_db)

# Shapes
cur.execute("CREATE TABLE shapes (shape_id integer,shape_pt_lat,shape_pt_lon,shape_pt_sequence integer);")
file_path = os.path.join(STM_DIR, 'shapes.txt')
with open(file_path,'r') as fin:
    dr = csv.DictReader(fin) # comma is default delimiter    
    #for i in dr:
        #print(i)
    shapes_db = [(int(i['shape_id']),i['shape_pt_lat'],i['shape_pt_lon'],int(i['shape_pt_sequence'])) for i in dr]
    
cur.executemany("INSERT INTO shapes (shape_id,shape_pt_lat,shape_pt_lon,shape_pt_sequence) VALUES (?, ?, ?, ?);", shapes_db)

# Services
cur.execute("CREATE TABLE services (service_id,monday,tuesday,wednesday,thursday,friday,saturday,sunday,start_date,end_date);")
file_path = os.path.join(STM_DIR, 'calendar.txt')
with open(file_path,'r') as fin:
    dr = csv.DictReader(fin) # comma is default delimiter    
    #for i in dr:
        #print(i)
    services_db = [(i['service_id'],i['monday'],i['tuesday'],i['wednesday'],i['thursday'],i['friday'],i['saturday'],i['sunday'],i['start_date'],i['end_date']) for i in dr]
    
cur.executemany("INSERT INTO services (service_id,monday,tuesday,wednesday,thursday,friday,saturday,sunday,start_date,end_date) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);", services_db)

# Trips
cur.execute("CREATE TABLE trips (route_id integer,service_id,trip_id integer PRIMARY KEY,trip_headsign,direction_id,shape_id integer,wheelchair_accessible,note_fr,note_en);")
file_path = os.path.join(STM_DIR, 'trips.txt')
with open(file_path,'r') as fin:
    dr = csv.DictReader(fin) # comma is default delimiter    
    #for i in dr:
        #print(i)
    trips_db = [(int(i['route_id']),i['service_id'],int(i['trip_id']),i['trip_headsign'],i['direction_id'],int(i['shape_id']),i['wheelchair_accessible'],i['note_fr'],i['note_en']) for i in dr]
    
cur.executemany("INSERT INTO trips (route_id,service_id,trip_id,trip_headsign,direction_id,shape_id,wheelchair_accessible,note_fr,note_en) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);", trips_db)

# Stop times
cur.execute("CREATE TABLE stop_times (trip_id integer,arrival_time,departure_time,stop_id,stop_sequence integer);")
file_path = os.path.join(STM_DIR, 'stop_times1.txt')
with open(file_path,'r') as fin:
    dr = csv.DictReader(fin) # comma is default delimiter    
    #for i in dr:
        #print(i)
    stop_times_db = [(int(i['trip_id']),i['arrival_time'],i['departure_time'],i['stop_id'],int(i['stop_sequence'])) for i in dr]
    
cur.executemany("INSERT INTO stop_times (trip_id,arrival_time,departure_time,stop_id,stop_sequence) VALUES (?, ?, ?, ?, ?);", stop_times_db)

file_path = os.path.join(STM_DIR, 'stop_times2.txt')
with open(file_path,'r') as fin:
    dr = csv.DictReader(fin) # comma is default delimiter    
    stop_times_db = [(int(i['trip_id']),i['arrival_time'],i['departure_time'],i['stop_id'],int(i['stop_sequence'])) for i in dr]
    
cur.executemany("INSERT INTO stop_times (trip_id,arrival_time,departure_time,stop_id,stop_sequence) VALUES (?, ?, ?, ?, ?);", stop_times_db)

file_path = os.path.join(STM_DIR, 'stop_times3.txt')
with open(file_path,'r') as fin:
    dr = csv.DictReader(fin) # comma is default delimiter    
    stop_times_db = [(int(i['trip_id']),i['arrival_time'],i['departure_time'],i['stop_id'],int(i['stop_sequence'])) for i in dr]
    
cur.executemany("INSERT INTO stop_times (trip_id,arrival_time,departure_time,stop_id,stop_sequence) VALUES (?, ?, ?, ?, ?);", stop_times_db)

con.commit()
con.close()