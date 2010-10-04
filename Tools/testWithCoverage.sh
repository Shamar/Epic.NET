#!/bin/bash
mono --debug --profile=monocov:outfile=/tmp/Challenge00.cov,+[Challenge00.DDDSample],+[Challenge00.DDDSample.Default] ../Challenges/Challenge00.DDDSample/UnitTests/NUnitRunner/bin/Debug/NUnitRunner.exe ../Challenges/Challenge00.DDDSample/All.nunit -run && monocov /tmp/Challenge00.cov
