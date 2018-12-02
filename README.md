# SEO Analyser
Web application that performs a simple SEO analysis of the text. User submits a text in English or URL, page filters out stop-words (e.g. ‘or’, ‘and’, ‘a’, ‘the’ etc), calculates number of occurrences on the page of each word, number of occurrences on the page of each word listed in meta tags, number of external links in the text.

# Technology
ASP.NET MVC 5 using C#,
.NET Framework 4.5.2

# Plugin
Bootstrap,
JQuery,
JQuery Datatable

# Design flow
Start -> User key-in Text or URL -> User submit form -> Web application get content from URL or the input text -> Remove stop-words from content if applicable -> Generate list of words from content if applicable -> Generate list of words from content meta tag if applicable -> Generate list of external links from content if applicable -> End

# Continuous Deployment
For ease of testing on actual environment, this web application has been integrated with the Azure Cloud Service to perform automated deployment. Feel free to browse through the public site at http://seoanalyser.azurewebsites.net/
