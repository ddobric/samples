library(shiny)
library(leaflet)

shinyUI(fluidPage(
    titlePanel("Predicted stats for hitters by position"),

    sidebarLayout(
        sidebarPanel(
            helpText("Show predicted stats for hitter"),
            uiOutput("controls")
        ),
        mainPanel(
            titlePanel("Position Players"),
            dataTableOutput("table"),
            titlePanel("Starting Pitchers"),
            dataTableOutput("starters"),
            titlePanel("Closers"),
            dataTableOutput("closers")
        )
    )
))