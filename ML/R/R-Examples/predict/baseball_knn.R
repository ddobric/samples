# Read all of the data into global variables

library(dplyr)

batting <- read.csv("../batting.csv", stringsAsFactors = FALSE)
pitching <- read.csv("../pitching.csv", stringsAsFactors = FALSE)
player_names <- read.csv("../master.csv", stringsAsFactors = FALSE)
fielding <- read.csv("../fielding.csv", stringsAsFactors = FALSE)

# Compute globals for rollup pitching and batting statistics
# We don't care about teams, so we need to compute the rollup for each year

rollup_pitching <- pitching %>%
    group_by(yearID, playerID) %>%
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
        WHIP = (BB + H) / (IPouts / 3),
        ID = sprintf("%s-%s", playerID, yearID)
    ) %>%
    as.data.frame()

rollup_batting <- batting %>%
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
        S = H - (X2B + X3B + HR),
        TB = S + 2 * X2B + 3 * X3B + 4 * HR,
        SLG = TB / AB,
        # BUG in R? If we try to call as.factor() on sprintf() below it 
        # winds up with all of the yearIDs being the same year in the factors (1984)
        ID = sprintf("%s-%s", playerID, yearID)) %>%
    as.data.frame()

get_intersection_by_playerid <- function(lhs, rhs) {
    player_ids <- intersect(
        lhs %>% select(playerID),
        rhs %>% select(playerID)
    )

    list(
        lhs = lhs %>% inner_join(player_ids, by = "playerID"),
        rhs = rhs %>% inner_join(player_ids, by = "playerID")
    )
}

predict_pitching_statistics <- function(prediction_year, years_to_train, minimum_game_threshold) {
    start_training_year = prediction_year - years_to_train

    training_set <- rollup_pitching %>%
        filter(yearID >= start_training_year & yearID < prediction_year & G > minimum_game_threshold)

    testing_set <- rollup_pitching %>%
        filter(yearID == prediction_year & G > minimum_game_threshold)

    answer_set <- rollup_pitching %>%
        filter(yearID == prediction_year + 1 & G > minimum_game_threshold)

    result <- get_intersection_by_playerid(testing_set, answer_set)
    testing_set <- result$lhs
    answer_set <- result$rhs

    # Extract just the features that we want by excluding
    # first two and last columns
    cols_to_select <- ncol(training_set) - 1
    training <- training_set[3:cols_to_select]
    testing <- testing_set[3:cols_to_select]
    answer <- answer_set[3:cols_to_select]

    # Now train the model using knn
    library(class)
    results <- knn(training, testing, training_set$ID, k = 3)

    # Construct a table that contains tuples of (playerID, predictedPlayerID)
    similarity_results <- data.frame(
        playerID = testing_set$playerID,
        stringsAsFactors = FALSE)
    similarity_results$referenceID = as.character(results)

    similarity_stats <- similarity_results %>%
        inner_join(player_names, by = "playerID") %>%
        inner_join(testing_set, by = "playerID") %>%
        inner_join(training_set, by = c("referenceID" = "ID")) %>%
        select(
            FirstName = nameFirst,
            LastName = nameLast,
            PlayerID = playerID.x,
            W = W.x,
            L = L.x,
            G = G.x,
            GS = GS.x,
            SV = SV.x,
            H = H.x,
            BB = BB.x,
            ER = ER.x,
            SO = SO.x,
            ERA = ERA.x,
            WHIP = WHIP.x,
            ReferenceID = playerID.y,
            YearID = yearID.y,
            W.p = W.y,
            L.p = L.y,
            G.p = G.y,
            GS.p = GS.y,
            SV.p = SV.y,
            H.p = H.y,
            BB.p = BB.y,
            ER.p = ER.y,
            SO.p = SO.y,
            ERA.p = ERA.y,
            WHIP.p = WHIP.y
        )

    similarity_stddev <- data.frame(
        W.sd = sd(similarity_stats$W - similarity_stats$W.p),
        L.sd = sd(similarity_stats$L - similarity_stats$L.p),
        G.sd = sd(similarity_stats$G - similarity_stats$G.p),
        GS.sd = sd(similarity_stats$GS - similarity_stats$GS.p),
        SV.sd = sd(similarity_stats$SV - similarity_stats$SV.p),
        H.sd = sd(similarity_stats$H - similarity_stats$H.p),
        BB.sd = sd(similarity_stats$BB - similarity_stats$BB.p),
        ER.sd = sd(similarity_stats$ER - similarity_stats$ER.p),
        SO.sd = sd(similarity_stats$SO - similarity_stats$SO.p),
        ERA.sd = sd(similarity_stats$ERA - similarity_stats$ERA.p),
        WHIP.sd = sd(similarity_stats$WHIP - similarity_stats$WHIP.p)
    )
    
    prediction_stats <- similarity_stats %>%
        select(FirstName, LastName, PlayerID, ReferenceID, YearID) %>%
        mutate(YearID = YearID + 1) %>%
        inner_join(answer_set, by = c("PlayerID" = "playerID")) %>%
        inner_join(training_set, by = c("ReferenceID" = "playerID", "YearID" = "yearID")) %>%
        select(
            FirstName,
            LastName,
            PlayerID,
            W = W.x,
            L = L.x,
            G = G.x,
            GS = GS.x,
            SV = SV.x,
            H = H.x,
            BB = BB.x,
            ER = ER.x,
            SO = SO.x,
            ERA = ERA.x,
            WHIP = WHIP.x,
            ReferenceID,
            YearID,
            W.p = W.y,
            L.p = L.y,
            G.p = G.y,
            GS.p = GS.y,
            SV.p = SV.y,
            H.p = H.y,
            BB.p = BB.y,
            ER.p = ER.y,
            SO.p = SO.y,
            ERA.p = ERA.y,
            WHIP.p = WHIP.y
        )

    # Compute standard deviation in difference between actual and prediction
    prediction_stddev <- data.frame(
        W.sd = sd(prediction_stats$W - prediction_stats$W.p),
        L.sd = sd(prediction_stats$L - prediction_stats$L.p),
        G.sd = sd(prediction_stats$G - prediction_stats$G.p),
        GS.sd = sd(prediction_stats$GS - prediction_stats$GS.p),
        SV.sd = sd(prediction_stats$SV - prediction_stats$SV.p),
        H.sd = sd(prediction_stats$H - prediction_stats$H.p),
        BB.sd = sd(prediction_stats$BB - prediction_stats$BB.p),
        ER.sd = sd(prediction_stats$ER - prediction_stats$ER.p),
        SO.sd = sd(prediction_stats$SO - prediction_stats$SO.p),
        ERA.sd = sd(prediction_stats$ERA - prediction_stats$ERA.p),
        WHIP.sd = sd(prediction_stats$WHIP - prediction_stats$WHIP.p)
    )

    # Construct list with results - predictions and standard deviation of the predictions
    list(predictions = prediction_stats, stddev = prediction_stddev)
}

# Parameters:
# 1. Year to predict
# 2. Year to compare against
# 3. Minimum number of games played to consider
# Returns: list containing 
#   data frame with predictions
#   data frame with standard deviation of predictions from actuals
# Algorithm:
# Simplest possible scenario is
# 1. Single-season performances only
# 2. Restrict to players who play more than 120 games in season
# 3. Only offensive stats. H, R, RBI, 2B, 3B, HR

predict_batting_statistics <- function(prediction_year, years_to_train, minimum_game_threshold) {
    start_training_year = prediction_year - years_to_train

    training_set <- rollup_batting %>%
        filter(yearID >= start_training_year & yearID < prediction_year & G > minimum_game_threshold)

    testing_set <- rollup_batting %>%
        filter(yearID == prediction_year & G > minimum_game_threshold)

    answer_set <- rollup_batting %>%
        filter(yearID == prediction_year + 1 & G > minimum_game_threshold)

    result <- get_intersection_by_playerid(testing_set, answer_set)
    testing_set <- result$lhs
    answer_set <- result$rhs

    # Extract just the features that we want by excluding
    # first two and last columns
    cols_to_select <- ncol(training_set) - 1
    training <- training_set[3:cols_to_select]
    testing <- testing_set[3:cols_to_select]
    answer <- answer_set[3:cols_to_select]

    # Now train the model using knn
    library(class)
    results <- knn(training, testing, training_set$ID, k = 3)

    # Construct a table that contains tuples of (playerID, predictedPlayerID)
    similarity_results <- data.frame(
        playerID = testing_set$playerID,
        stringsAsFactors = FALSE)
    similarity_results$referenceID = as.character(results)

    # Construct a comparison table that contains stats for playerID and stats for predictedPlayerID
    similarity_stats <- similarity_results %>%
        inner_join(player_names, by = "playerID") %>%
        inner_join(testing_set, by = "playerID") %>%
        inner_join(training_set, by = c("referenceID" = "ID")) %>%
        select(
            FirstName = nameFirst,
            LastName = nameLast,
            PlayerID = playerID.x,
            G = G.x,
            AB = AB.x,
            R = R.x,
            H = H.x,
            X2B = X2B.x,
            X3B = X3B.x,
            HR = HR.x,
            RBI = RBI.x,
            SO = SO.x,
            SB = SB.x,
            BB = BB.x,
            ReferenceID = playerID.y,
            YearID = yearID.y,
            G.p = G.y,
            AB.p = AB.y,
            R.p = R.y,
            H.p = H.y,
            X2B.p = X2B.y,
            X3B.p = X3B.y,
            HR.p = HR.y,
            RBI.p = RBI.y,
            SO.p = SO.y,
            SB.p = SB.y,
            BB.p = BB.y)

    # Compute standard deviation in difference between player and similar
    similarity_stddev <- data.frame(
        G.sd = sd(similarity_stats$G - similarity_stats$G.p),
        AB.sd = sd(similarity_stats$AB - similarity_stats$AB.p),
        R.sd = sd(similarity_stats$R - similarity_stats$R.p),
        H.sd = sd(similarity_stats$H - similarity_stats$H.p),
        X2B.sd = sd(similarity_stats$X2B - similarity_stats$X2B.p),
        X3B.sd = sd(similarity_stats$X3B - similarity_stats$X3B.p),
        HR.sd = sd(similarity_stats$HR - similarity_stats$HR.p),
        RBI.sd = sd(similarity_stats$RBI - similarity_stats$RBI.p),
        SO.sd = sd(similarity_stats$SO - similarity_stats$SO.p),
        SB.sd = sd(similarity_stats$SB - similarity_stats$SB.p),
        BB.sd = sd(similarity_stats$BB - similarity_stats$BB.p))

    # Generate prediction stats by taking stats from the reference player's following year
    prediction_stats <- similarity_stats %>%
        select(FirstName, LastName, PlayerID, ReferenceID, YearID) %>%
        mutate(YearID = YearID + 1) %>%
        inner_join(answer_set, by = c("PlayerID" = "playerID")) %>%
        inner_join(training_set, by = c("ReferenceID" = "playerID", "YearID" = "yearID")) %>%
        select(
            FirstName,
            LastName,
            PlayerID,
            G = G.x,
            AB = AB.x,
            R = R.x,
            H = H.x,
            X2B = X2B.x,
            X3B = X3B.x,
            HR = HR.x,
            RBI = RBI.x,
            SO = SO.x,
            SB = SB.x,
            BB = BB.x,
            ReferenceID,
            YearID,
            G.p = G.y,
            AB.p = AB.y,
            R.p = R.y,
            H.p = H.y,
            X2B.p = X2B.y,
            X3B.p = X3B.y,
            HR.p = HR.y,
            RBI.p = RBI.y,
            SO.p = SO.y,
            SB.p = SB.y,
            BB.p = BB.y)

    # Compute standard deviation in difference between actual and prediction
    prediction_stddev <- data.frame(
        G.sd = sd(prediction_stats$G - prediction_stats$G.p),
        AB.sd = sd(prediction_stats$AB - prediction_stats$AB.p),
        R.sd = sd(prediction_stats$R - prediction_stats$R.p),
        H.sd = sd(prediction_stats$H - prediction_stats$H.p),
        X2B.sd = sd(prediction_stats$X2B - prediction_stats$X2B.p),
        X3B.sd = sd(prediction_stats$X3B - prediction_stats$X3B.p),
        HR.sd = sd(prediction_stats$HR - prediction_stats$HR.p),
        RBI.sd = sd(prediction_stats$RBI - prediction_stats$RBI.p),
        SO.sd = sd(prediction_stats$SO - prediction_stats$SO.p),
        SB.sd = sd(prediction_stats$SB - prediction_stats$SB.p),
        BB.sd = sd(prediction_stats$BB - prediction_stats$BB.p))

    # Outfielders are called LF, RF, CF and OF. Normalize all of them to be OF and collapse
    normalized_fielding <- fielding %>%
        mutate(POS = ifelse(POS == "LF" | POS == "RF" | POS == "CF", "OF", POS))

    # Compute table of eligible players by position and their predicted stats
    eligible_hitters_stats_by_position <- normalized_fielding %>%
        filter(yearID == prediction_year) %>%
        select(playerID, POS) %>%
        inner_join(prediction_stats, by = c("playerID" = "PlayerID")) %>%
        select(
            FirstName,
            LastName,
            POS,
            G = G.p,
            AB = AB.p,
            R = R.p,
            H = H.p,
            X2B = X2B.p,
            X3B = X3B.p,
            HR = HR.p,
            RBI = RBI.p,
            SO = SO.p,
            SB = SB.p,
            BB = BB.p
        ) %>%
        distinct(
            FirstName,
            LastName,
            POS
        )

    # Construct list with results - predictions and standard deviation of the predictions
    list(predictions = eligible_hitters_stats_by_position, stddev = prediction_stddev)
}

# Simplest possible algorithm is to assume next year is same as this year
predict_batting_statistics_naive <- function(prediction_year, minimum_game_threshold) {

    testing_set <- rollup_batting %>%
        filter(yearID == prediction_year & G > minimum_game_threshold)

    answer_set <- rollup_batting %>%
        filter(yearID == prediction_year + 1 & G > minimum_game_threshold)

    result <- get_intersection_by_playerid(testing_set, answer_set)
    testing_set <- result$lhs
    answer_set <- result$rhs

    prediction_stats <- testing_set %>%
        inner_join(player_names, by = "playerID") %>%
        inner_join(answer_set, by = "playerID") %>%
        select(
            FirstName = nameFirst,
            LastName = nameLast,
            PlayerID = playerID,
            G = G.x,
            AB = AB.x,
            R = R.x,
            H = H.x,
            X2B = X2B.x,
            X3B = X3B.x,
            HR = HR.x,
            RBI = RBI.x,
            SO = SO.x,
            SB = SB.x,
            BB = BB.x,
            G.p = G.y,
            AB.p = AB.y,
            R.p = R.y,
            H.p = H.y,
            X2B.p = X2B.y,
            X3B.p = X3B.y,
            HR.p = HR.y,
            RBI.p = RBI.y,
            SO.p = SO.y,
            SB.p = SB.y,
            BB.p = BB.y)

    # Compute standard deviation in difference between actual and prediction
    prediction_stddev <- data.frame(
        G.sd = sd(prediction_stats$G - prediction_stats$G.p),
        AB.sd = sd(prediction_stats$AB - prediction_stats$AB.p),
        R.sd = sd(prediction_stats$R - prediction_stats$R.p),
        H.sd = sd(prediction_stats$H - prediction_stats$H.p),
        X2B.sd = sd(prediction_stats$X2B - prediction_stats$X2B.p),
        X3B.sd = sd(prediction_stats$X3B - prediction_stats$X3B.p),
        HR.sd = sd(prediction_stats$HR - prediction_stats$HR.p),
        RBI.sd = sd(prediction_stats$RBI - prediction_stats$RBI.p),
        SO.sd = sd(prediction_stats$SO - prediction_stats$SO.p),
        SB.sd = sd(prediction_stats$SB - prediction_stats$SB.p),
        BB.sd = sd(prediction_stats$BB - prediction_stats$BB.p))

    list(predictions = prediction_stats, stddev = prediction_stddev)
}

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

prediction_year <- 2014
years_to_train <- 30
minimum_game_threshold <- 80

predict_batting_statistics_3 <- function(prediction_year, years_to_train, minimum_game_threshold) {

    training_set <- read.csv("../three_year_batting.csv", stringsAsFactors = FALSE) %>%
        filter(yearID <= (prediction_year - 3)) %>%
        mutate(ID = sprintf("%s-%s", playerID, yearID)) %>%
        select(yearID, playerID, ID, 4:51)

    candidates <- rollup_batting %>%
        filter(yearID > (prediction_year - 3) & yearID <= prediction_year & G > minimum_game_threshold) %>%
        group_by(playerID) %>%
        tally() %>%
        filter(n == 3)

    answer_set <- rollup_batting %>%
        filter(yearID == prediction_year + 1 & G > minimum_game_threshold)

    # Compute the interesection of playerIDs between test and answer set
    results <- get_intersection_by_playerid(candidates, answer_set)
    answer_set <- results$rhs

    # Grab 3 year stats for testing set
    stats <- rollup_batting %>%
        filter(yearID > (prediction_year - 3) & yearID <= prediction_year) %>%
        inner_join(candidates, by = "playerID") %>%
        select(1:18)

    testing_set <- data.frame()
    for (id in answer_set$playerID) {
        player <- stats %>% filter(playerID == id)
        s1 <- player %>% slice(1)
        s2 <- player %>% slice(2) %>% select(3:18)
        s3 <- player %>% slice(3) %>% select(3:18)
        z1 <- cbind(s1, s2, s3)
        testing_set <- rbind(testing_set, z1)
    }

    names(testing_set) <- c("yearID", "playerID", "G.x", "AB.x", "R.x", "H.x",
        "X2B.x", "X3B.x", "HR.x", "RBI.x", "SO.x", "SB.x",
        "CS.x", "BB.x", "AVG.x", "S.x", "TB.x", "SLG.x",
        "G.y", "AB.y", "R.y", "H.y", "X2B.y", "X3B.y",
        "HR.y", "RBI.y", "SO.y", "SB.y", "CS.y", "BB.y",
        "AVG.y", "S.y", "TB.y", "SLG.y", "G.z", "AB.z",
        "R.z", "H.z", "X2B.z", "X3B.z", "HR.z", "RBI.z",
        "SO.z", "SB.z", "CS.z", "BB.z", "AVG.z", "S.z",
        "TB.z", "SLG.z")

    # Extract just the features that we want by excluding
    training <- training_set[4:length(training_set)]
    testing <- testing_set[3:length(testing_set)]
    answer <- answer_set[3:length(answer_set)]

    # Now train the model using knn
    library(class)
    results <- knn(training, testing, training_set$ID, k = 3)

    # Construct a table that contains tuples of (playerID, predictedPlayerID)
    similarity_results <- data.frame(
        playerID = testing_set$playerID,
        stringsAsFactors = FALSE)
        similarity_results$referenceID = as.character(results)

    # Construct 
    references <- strsplit(similarity_results$referenceID, "-") %>% as.data.frame(stringsAsFactors = FALSE)
    references <- t(references) %>% as.data.frame(stringsAsFactors = FALSE)
    names(references) <- c("referenceID.y", "yearID")
    references$ID <- similarity_results$referenceID
    references$yearID <- as.numeric(references$yearID)

    # Join
    prediction_stats <- similarity_results %>%
        inner_join(references, by = c("referenceID" = "ID")) %>%
        inner_join(player_names, by = "playerID") %>%
        inner_join(answer_set, by = "playerID") %>%
        mutate(yearID.x = yearID.x + 1) %>%
        inner_join(rollup_batting, by = c("referenceID.y" = "playerID", "yearID.x" = "yearID")) %>%
        select(
            FirstName = nameFirst,
            LastName = nameLast,
            PlayerID = playerID,
                G = G.x,
                AB = AB.x,
                R = R.x,
                H = H.x,
                X2B = X2B.x,
                X3B = X3B.x,
                HR = HR.x,
                RBI = RBI.x,
                SO = SO.x,
                SB = SB.x,
                BB = BB.x,
                ReferenceID = referenceID.y,
                YearID = yearID.x,
                G.p = G.y,
                AB.p = AB.y,
                R.p = R.y,
                H.p = H.y,
                X2B.p = X2B.y,
                X3B.p = X3B.y,
                HR.p = HR.y,
                RBI.p = RBI.y,
                SO.p = SO.y,
                SB.p = SB.y,
                BB.p = BB.y)

    # Compute standard deviation in difference between actual and prediction
    prediction_stddev <- data.frame(
            G.sd = sd(prediction_stats$G - prediction_stats$G.p),
            AB.sd = sd(prediction_stats$AB - prediction_stats$AB.p),
            R.sd = sd(prediction_stats$R - prediction_stats$R.p),
            H.sd = sd(prediction_stats$H - prediction_stats$H.p),
            X2B.sd = sd(prediction_stats$X2B - prediction_stats$X2B.p),
            X3B.sd = sd(prediction_stats$X3B - prediction_stats$X3B.p),
            HR.sd = sd(prediction_stats$HR - prediction_stats$HR.p),
            RBI.sd = sd(prediction_stats$RBI - prediction_stats$RBI.p),
            SO.sd = sd(prediction_stats$SO - prediction_stats$SO.p),
            SB.sd = sd(prediction_stats$SB - prediction_stats$SB.p),
            BB.sd = sd(prediction_stats$BB - prediction_stats$BB.p))

    list(predictions = prediction_stats, stddev = prediction_stddev)
}