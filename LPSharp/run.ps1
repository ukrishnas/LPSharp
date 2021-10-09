# Read unit test MPS files.
$testdata = "c:\users\krish\work\LPSharp\UnitTests\TestData"
read-mps $testdata\test1.mps
read-mps $testdata\test1.mps.gz
read-mps $testdata\test2.mps
read-mps $testdata\test3.mps
read-mps $testdata\hello.mps

# Read MSF MPS files.
$eleval = "c:\users\krish\work\eleval\data\MsfModels"
read-mps "$eleval\model_2021-10-07T06.38.20.103Z.mps.gz" -key mincost
read-mps "$eleval\model_2021-10-07T06.38.31.691Z.mps.gz" -key diverse
read-mps "$eleval\model_2021-10-07T06.38.45.158Z.mps.gz" -key minutil
