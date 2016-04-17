# R Works great with SQL data
library(RODBC)

# Specify a system/user data source name
conn <- odbcConnect("baseball")

# Dump the tables in the database into a dataframe
tables <- sqlTables(conn)

# Perform a dynamic SQL query
batters <- sqlQuery(conn, "select * from Batting", stringsAsFactors = FALSE)

# Examine it in Variable Explorer
# Examine it in Table Viewer
# Examine the in the console summary 
# Note how the maximum values are accurate if you know your BB stats
summary(batters)

# Let's take a closer look at the data
SQL <- "select * from Batting where HR = 73"
hr_leader <- sqlQuery(conn, SQL)
hr_leader

# We can, of course join using SQL
SQL <- "select * from Batting as b inner join Master as p on b.playerID = p.playerID where HR = 73"
hr_leader_details <- sqlQuery(conn, SQL)
hr_leader_details

# We can do joins locally too
master <- sqlQuery(conn, "select * from Master", stringsAsFactors = FALSE)

# dplyr "d plyer" is a data manipulation library
library(dplyr)
hr_local_join <- batters %>%
    inner_join(master, by = "playerID") %>%
    filter(HR == 73)
hr_local_join

# Let's take a closer look at the batting data
kenny_lofton <- batters %>%
    filter(playerID == "loftoke01")
kenny_lofton

# Notice that there are column names that start with a number
# e.g., 2B and 3B. These are not valid R identifiers, so we 
# need to rename them
rename_col <- function(df, old_name, new_name) {
    names(df)[names(df) == old_name] <- new_name
    df
}

fixup_df_names <- function(df) {
    names(df)[names(df) == "2B"] <- "X2B"
    names(df)[names(df) == "3B"] <- "X3B"
    df
}

kenny_lofton <- fixup_df_names(kenny_lofton)

# We can see that if we care about yearly statistics that we need
# to do some summarization of the data frame
rollup <- function(df) {
    result <- df %>%
        group_by(playerID, yearID) %>%
        summarize(
            G = sum(G), AB = sum(AB), R = sum(R), H = sum(H), H2B = sum(X2B), H3B = sum(X3B), HR = sum(HR), RBI = sum(RBI), SB = sum(SB), CS = sum(CS), BB = sum(BB), SO = sum(SO), IBB = sum(IBB), HBP = sum(HBP), SH = sum(SH), SF = sum(SF), GIDP = sum(GIDP)) %>%
        as.data.frame() 
    result
}

kenny_lofton_annual <- rollup(kenny_lofton)
kenny_lofton_annual

# Looking at tables on a console is OK, but there are better ways
# Let's visualize it using an interactive chart
library(DT)
datatable(kenny_lofton)

# Now let's apply rollup against all batting stats
batters <- fixup_df_names(batters)
batters <- rollup(batters)

# So far, we have been looking at R as an equivalence class of SQL
# Though a) slower, b) more limited size of data sets. 
# Let's look at it through a new lens: visualization in next section
