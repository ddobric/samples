# A look at idiosyncracies of the R language

# Variable assignment happens with <-
a <- 3

# It also works with = (and used to work with _)
b = 3
a + b

# There are no value types!
# Oh, and "." is not an operator
is.vector(a)
length(a)

# Vectors are homogenous collections
# Create vectors using the c() combine function
v1 <- c("a", "b", "c")
class(v1)
is.vector(v1)

# Index entries in vectors - note 1-based indexing
v1[1]
v1[2]
v1[3]

v2 <- c(1,2,3)
class(v2)
is.vector(v2)

# Vectors can have "names" 
names(v2) <- c("one", "two", "three")
v2

# Can examine the structure of an object via str()
str(v2)

# Vectors are homogeneous, so implicit conversions happen
v3 <- c(1, "2", 3)
v3
class(v3)

# Lists are heterogeneous collections
l1 <- list(1, "2", 3)
l1

# Remember that it is a list of vectors
is.vector(l1[1])
class(l1[[1]])

# Dataframes are a collection of dataframes
# mtcars is a built-in dataset
df1 <- mtcars
class(df1)
str(df1)
class(df1[1])