# This script will subset the Lahman database such that we retain only the data for the past 3 years

batting <- read.csv("batting.csv", stringsAsFactors = FALSE)
pitching <- read.csv("pitching.csv", stringsAsFactors = FALSE)
player_names <- read.csv("master.csv", stringsAsFactors = FALSE)
fielding <- read.csv("fielding.csv", stringsAsFactors = FALSE)

library(dplyr)

# Compute total statistics for all teams that a player hit for that year
rollup_batting <- batting %>%
    filter(yearID == 2015 & AB > 0) %>%
    group_by(playerID) %>%
    summarize(
        G = sum(G),
        AB = sum(AB),
        R = sum(R),
        H = sum(H),
        X2B = sum(X2B),
        X3B = sum(X3B),
        HR = sum(HR),
        RBI = sum(RBI),
        SB = sum(SB),
        CS = sum(CS),
        BB = sum(BB)
    ) %>%
    mutate(
        AVG = H / AB,
        S = H - (X2B + X3B + HR), 
        TB = S + 2 * X2B + 3 * X3B + 4 * HR,
        SLG = TB / AB
    )

eligible_hitters_by_position <- fielding %>%
    filter(yearID == 2015 & G > 20 & POS != "P") %>%
    select(playerID, POS) %>%
    inner_join(player_names, by = "playerID") %>%
    select(playerID, FirstName = nameFirst, LastName = nameLast, POS)

# Compute total statistics for all teams that a player pitched for that year
rollup_pitching <- pitching %>%
    filter(yearID == 2015) %>%
    group_by(playerID) %>%
    summarize(
        W = sum(W),
        L = sum(L),
        G = sum(G),
        GS = sum(GS),
        SV = sum(SV),
        H = sum(H),
        BB = sum(BB),
        ER = sum(ER),
        IPouts = sum(IPouts),
        SO = sum(SO)
    ) %>%
    mutate(
        ERA = ER / (IPouts / 3 / 9),
        WHIP = (BB + H) / (IPouts / 3)
    ) %>%
    inner_join(player_names) %>%
    select(playerID, FirstName = nameFirst, LastName = nameLast, W, L, G, GS, SV, H, BB, ER, IPouts, SO, ERA, WHIP) 