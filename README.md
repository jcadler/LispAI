I split the AI into two sections, what I like to call the "front end" and the "back end". The "front end" consists of the C# code which will grab the article
s from the Wikipedia API and then parse them. By parsing them the "front end" gets rid of all extraneouse XML info (as the articles are grabbed in an XML for
mat), identifies which words have a links to other articles (what I like to call "hrefs", which is from the XML syntax for a hyperlink) and keeps that "href"
 tag near the word. The "front end" then uses an API for Princeton's Word Net which get the "types" for each individual word (noun, verb, adjective etc.) pla
cing the type in parentheses next to the word.The front end then transfers the parsed article to the "back end" using FTP. I was running my Lisp code on a Ub
untu Linux desktop and my C# on my Vista laptop so I used FTP to pass queries from the AI and results from the "front end" back and forth. The AI then takes 
the articles, which are stored in text files, puts them into local memory and runs through them, seperating the words and setting up lists to make AI analysi
s easier (i.e. each word is in a list with its "type" and "href" if it has a reference). The AI would then use the list of words and info to send queries for
 backlinks, searching the backlink article for information that is similar to the first article, ultimately determining what information is "important" based
 off of the results. Afterwords, the "back end" would return the results to the "front end" which would then display them in a windows form and save them to 
a file.