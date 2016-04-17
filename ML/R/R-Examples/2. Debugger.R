f1 <- function(x) {
    x <- x + 1
    print(x)
    x
}

f2 <- function(x) {
    f1(x + 1)
}

f2(42)