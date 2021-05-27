## Tools for Seismic Data Processing (OpendTect and C-View Processing)

Converting exchange text files between EGS Inhouse software : C-View Processing, C-View Bathy and OpendTect

REF file format:
```
Trace# Easting Northing H0 H1 H2 ... H10
H0 = Water depth (m)
H1 H2 .. = Depth below H0
```

DAT file format: (OpendTect 2D Horizon)
```
Linename Easting Northing SP# Trace# Depth(TWT in ms)
```

RXY
```
Trace# Easting Northing
```

RXYZ
```
Trace# Easting Northing WaterDepth(m)
```

CNV files
```
dd/mm/yyyy HH:mm:ss.sss Trace# Easting Northing Gyro Tow_Hdg Fix Tracked_water_depth
The Trace# Easting Northing will be extracted.
```

PC files
```
Standard EGS Position Check file.
Trace# and PUFI coordinates in position check file will be used.
```

Drag in ...
```
CNV / PC files
to convert them to RXY files

RXYZ files
to linear interpolate the data gap (zero) in Z value
and make 'RXYZ Combination Output.txt' with SV 1530 m/s

DAT files (OpendTect 2D Horizon)
to reformat for SBP Horizon Proc.exe
nFix decimal and >9999 SP# exported by OpendTect v6.6


C-View Bathy v1.9.1 can update H0 in REF directly
Functions below were alternative method

REF file (1 file)
Extract the Trace# Easting and Northing to RXY file for CBG update

REF+RXYZ (2 file)
Update H0 in REF with Updated RXYZ (RXY originated from REF)
```
