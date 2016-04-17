library(shiny)
library(DT)
library(dplyr)

# This code runs exactly once. We call the function to compute the model

source("baseball_knn.R")
results <- predict_batting_statistics(2014, 30, 100)
predictions <- results$predictions

pitchers <- predict_pitching_statistics(2014, 30, 10)
starters <- pitchers$predictions %>%
    filter(GS > 10)
closers <- pitchers$predictions %>%
    filter(SV > 10)

positions = c("C", "1B", "2B", "3B", "SS", "OF")

shinyServer(function(input, output) {

    # We dynamically generate a listbox control for the client that 
    # contains the list of countries sorted alphabetically

    output$controls <- renderUI({
        selectInput("position",
            label = "Select a position",
            choices = positions, 
            selected = "C"
        )
    })

    output$table <- renderDataTable({
        datatable(
            predictions %>%
                filter(POS == input$position) %>% mutate(AVG = H/AB)
        ) %>%
        formatRound("AVG", digits = 3)
    })

    output$starters <- renderDataTable({
        datatable(starters) %>%
            formatRound(c("ERA", "WHIP", "ERA.p", "WHIP.p"), digits = c(2,3,2,3))
    })

    output$closers <- renderDataTable({
        datatable(closers) %>%
            formatRound(c("ERA", "WHIP", "ERA.p", "WHIP.p"), digits = c(2,3,2,3))
    })
})