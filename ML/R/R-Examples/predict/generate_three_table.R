# Experiments with random forest 

# Group By Player, Year
# Summarize - need to take groups of 3 year intervals and splat them
# horizontally

library(dplyr)

batting <- read.csv("../batting.csv", stringsAsFactors = FALSE)

# Start with rollup
rollup_batting <- batting %>%
    filter(G > 80 & AB > 0) %>%
    group_by(yearID, playerID) %>%
    summarize(
        G = sum(G),
        AB = sum(AB),
        R = sum(R),
        H = sum(H),
        X2B = sum(X2B),
        X3B = sum(X3B),
        HR = sum(HR),
        RBI = sum(RBI),
        SO = sum(SO),
        SB = sum(SB),
        CS = sum(CS),
        BB = sum(BB)) %>%
    mutate(
        AVG = H / AB,
        X1B = H - (X2B + X3B + HR),
        TB = X1B + 2 * X2B + 3 * X3B + 4 * HR,
        SLG = TB / AB
    ) %>%
    as.data.frame()

players_with_more_than_three_years <- rollup_batting %>%
    filter(yearID > 1980 & G > 80) %>%
    group_by(playerID) %>%
    tally() %>%
    filter(n > 3)

get_player_3_year_factors <- function(player) {
    result <- data.frame()
    for (i in 1:(length(player$yearID) - 3)) {
        s1 <- player %>% slice(i)
        s2 <- player %>% slice(i + 1) %>% select(3:18)
        s3 <- player %>% slice(i + 2) %>% select(3:18)
        z1 <- cbind(s1, s2, s3)
        result <- rbind(result, z1)
    }
    result
}

# Now try with all players
all_three_year_stats <- data.frame()
for (id in players_with_more_than_three_years$playerID) {
    print(id)
    player <- rollup_batting %>%
        filter(playerID == id)
    player_3 <- get_player_3_year_factors(player)
    all_three_year_stats <- rbind(all_three_year_stats, player_3)
}

write.csv(all_three_year_stats, "../three_year_batting.csv")