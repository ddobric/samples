# Lots of ways to get help in R

# We have both function and parameter IntelliSense ...

# Retrieve help for a specific function using ? operator
?read.csv

# Note that help works only for loaded packages
?select

library(dplyr)
?select

# Search all installed packages for help
??filter

# Packages come with vignettes
browseVignettes(package = "dplyr")

# Browse all vignettes
browseVignettes()

# Packages come with sample data
data(package="dplyr")

# Browse all sample data
data()