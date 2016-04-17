# Plot using plain-vanilla R

# Helper function to compute rollup statistics
rollup <- function(df) {
    result <- df %>%
        group_by(playerID, yearID) %>%
        summarize( 
            G = sum(G), AB = sum(AB), R = sum(R), H = sum(H), H2B = sum(X2B), H3B = sum(X3B), HR = sum(HR), RBI = sum(RBI), SB = sum(SB), CS = sum(CS), BB = sum(BB), SO = sum(SO), IBB = sum(IBB), HBP = sum(HBP), SH = sum(SH), SF = sum(SF), GIDP = sum(GIDP)) %>%
        as.data.frame()
    result
}

# Read data from CSV file (for a change)
# Introduce the file IntelliSense
batting <- read.csv("Batting.csv", stringsAsFactors = FALSE)



library(dplyr)

# Filter data to "real hitters" and rollup to annual stats
hitters <- batting %>%
    filter(AB > 50) %>%
    rollup() %>%
    select(H, AB) %>%
    sample_n(5000)

# Plot AB vs. H using built-in R plotting libraries
plot(hitters$AB, hitters$H, main = "AB vs H from 1871 - present", xlab = "AB", ylab = "H")

# Plot AB vs. H using ggplot with a smoothing line
library(ggplot2)
p <- ggplot(hitters, aes(AB, H)) +
    geom_point() +
    stat_smooth() +
    ggtitle("AB vs. H from 1871 - present")
p

# Generate exactly the same plot using plotly library
library(plotly)
ggplotly(p)

# Now let's use some features of plot.ly
plot_ly(hitters, x = AB, y = H, mode = "markers", color = H/AB)
