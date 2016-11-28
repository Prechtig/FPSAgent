#!/bin/sh

# Only keep score lines
grep Score $1 > grepped

# Clean score lines into “iteration \tab score”
sed -E "s/o.d\.o\.l\.ScoreIterationListener - Score at iteration ([0-9]+) is ([0-9]+\.[0-9]+)/\1     \2/g" grepped > cleaned

# Keep every n-th line
awk -v nth=$2 'NR%nth==1' cleaned > nth

# Insert headers
sed '1s/^/iteration  score\
/g' nth > $1.dat

# Remove tmp files
rm grepped
rm cleaned
rm nth