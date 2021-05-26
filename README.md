Tools for Seismic Data Processing (OpendTect and C-View Processing)

Converting exchange text files between EGS Inhouse software : C-View Processing, C-View Bathy and OpendTect

REF file format:
Trace# Easting Northing H0 H1 H2 ... H10
H0 = Water depth (m)
H1 H2 .. = Depth below H0

DAT file format: (OpendTect 2D Horizon)
Linename Easting Northing SP# Trace# Depth(TWT in ms)

RXY
Trace# Easting Northing

RXYZ
Trace# Easting Northing WaterDepth(m)

CNV files
dd/mm/yyyy HH:mm:ss.sss trace# Easting Northing Gyro Tow_Hdg Fix Tracked_water_depth

PC files
Trace# and PUFI coordinates in position check file will be used.
