# Run this file to install packages in the required_packages
# list. Will do the right thing if not present.

required_packages <- c(
                     "DT",
                     "dplyr",
                     "plotly",
                     "ggplot2",
                     "shiny"
                     )

install_package_if_necessary <- function(package) {
    if (!require(package, character.only = TRUE)) {
        install.packages(package, dep = TRUE)
    }
    suppressPackageStartupMessages(package)
}

install_required_packages <- function(required_packages) {
    lapply(required_packages, install_package_if_necessary)
}

invisible(install_required_packages(required_packages))
