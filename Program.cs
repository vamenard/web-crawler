using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///
/// <summary>
/// Web Search application displaying the next word of a search query.
/// Vincent Menard <vamenard@gmail.com>  2015-04-20
/// </summary>
///



namespace ConsoleApplication1
{
    class WebSearch
    {
        private Boolean htmlIndice;  // Used to know if within html tags < >
        private Boolean inWord;      // indicate if in process of lookup the next word
        private string nextWord;     // Word found after seach query
        private int resultCount;     // Track the number of search queries found
        private int wordsCount;      // Track the number of works found
        private int inResultLkp = 0; // Position of input lookup comparator head


        private char[] searchInput = "condimentum".ToCharArray();

        /// <summary>
        /// Validate url by instanciating new uri.
        /// </summary>
        
          
        public string validateURL( string url )
        {

            try
            {
                var uri = new Uri(url);
            }
            catch (UriFormatException)
            {
                Console.WriteLine("Please Enter a valid url!");
                return null;
            }

            return url;
        }


        /// <summary>
        /// Compare chars according to the html state
        /// </summary>


        public Boolean compareChar( Char input, Char search )
        {
            // Check for html tag
            if ((search == '<') || (search == '>'))
            {
                // Proper filtering would look for miss formatted or javascript
                htmlSwitch();
                return false;
            }

            if (!isHTML())
            {
                if (input == search)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Look for the next word in buffer by searching for a space
        /// </summary>

        public string findNextWord( int pos, Char[] read )
        {
            int itr = pos;
            while (itr < read.Length)
            {
                if (inWord && 
                    (read[itr] != ' ') && (read[itr] != '\n') && (read[itr] != '\r'))
                {
                    nextWord += read[itr].ToString();
                }
                else if (inWord && 
                    ((read[itr] == ' ') || (read[itr] == '\n') || (read[itr] == '\r')))
                {
                    inWord = false;
                    incWords();
                    return nextWord;
                }
                else if (!inWord && 
                    ((read[itr] == ' ') || (read[itr] == '\n') || (read[itr] == '\r')))
                {
                    inWord = true;
                }
                itr++;
            } // !while
            
            // Something went wrong, no word (space), long word or buffer ended before
            if (inWord)
            {
                return nextWord;  // Error: Word is partially retreived
            }
            return null;
        }

        /// <summary>
        /// Continue de next word search after reading buffer ended.
        /// </summary>
        
        public string completeNextWord( string nextWord, Char[] read ) {
            
            string completed = findNextWord(0, read);
            string finalWord = nextWord + completed;

            if ((!inWord) && (nextWord != null))
            {
                incWords();
                displayResult();
            }

            return finalWord;
        }

        /// <summary>
        /// Display the next word next to the query string as a result.
        /// </summary>
        
        private void displayResult() {
            
            Console.WriteLine("Result Found!");
            // Clean up pontuation
            Console.WriteLine(  "Result " + getResultCount() +
                                " Found - Word after query is: " + nextWord.Replace(".", "")); 
            nextWord = "";
            
        }

        /// <summary>
        /// Check and reset proper state when search is ended.
        /// </summary>
        
        public void validateGraceful()
        {
            if ((inWord) && (nextWord != null))
            {
                // The word you were in is the last word, display it
                incWords();
                displayResult();
            }
            else if (!inWord && (getWordsCount() != getResultCount()))
            {
                // There is no words after the search query
                Console.WriteLine("Result Found!");
                Console.WriteLine("Result " + getResultCount() + " Found - There is no word after the query string");
            }
            
            Console.WriteLine("\nSearch ended with " + getResultCount() + " occurence.");
            // reset for future object usage.
            nextWord = "";
            inWord = false;
            htmlIndice = false;
            resultCount = 0;
            wordsCount = 0;
            inResultLkp = 0;
            searchInput = null;

        }

        /// <summary>
        /// Set the object query string.
        /// </summary>

        public void setQuery(Char[] query)
        {
            searchInput = query;
        }

        /// <summary>
        /// Give the html state (read or ignore).
        /// </summary>

        public Boolean isHTML()
        {
            if (htmlIndice)
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// Change html state (read or ignore).
        /// </summary>
        
        private void htmlSwitch()
        {
            if (htmlIndice)
                htmlIndice = false;
            else
                htmlIndice = true;
        }

        /// <summary>
        /// Get the current number of results.
        /// </summary>

        public int getResultCount() {
            return resultCount;
        }

        /// <summary>
        /// Increment search results found.
        /// </summary>
        
        private void incResult()
        {
            resultCount = resultCount+1;
        }

        /// <summary>
        /// Get the number of words found .
        /// </summary>
        
        public int getWordsCount()
        {
            return wordsCount;
        }

        /// <summary>
        /// Increment the number of words.
        /// </summary>
        
        private void incWords()
        {
            wordsCount = wordsCount + 1;
        }

        /// <summary>
        /// Execute sequetial search and locate the next word, if the query search query
        /// is found. The state is saved between calls on this method. Three states
        /// are possibles: 1- query found 2- query found w/word 3- query found w/o word.
        /// </summary>
        
        public void sequentialSearch(Char[] read)
        {
            int pos=0;
            if (inWord)
            {
                // If the search for the next word is started but not completed
                completeNextWord( nextWord, read );
            }
            else if (!inWord && (getWordsCount() != getResultCount()))
            {
                // If the search for next word hasnt really started
                nextWord = findNextWord(pos, read);
                if ((!inWord) && (nextWord != null))
                {
                    displayResult();
                }
            }

            // We search for query by looping the char array.
            if (!inWord && (getWordsCount() == getResultCount()))
            {
                
                while (pos < read.Length)
                {
                    // Are char equals
                    if (compareChar(searchInput[inResultLkp], read[pos]))
                    {
                        // Are we looking for at last char
                        if (inResultLkp == (searchInput.Length - 1))
                        {
                            incResult();
                            // Find next word
                            nextWord = findNextWord((pos +1), read);
                            if ((!inWord) && (nextWord != null))
                            {
                                displayResult();
                            }
                            inResultLkp = 0;
                        }
                        else
                        {
                            // Keep track of the begining of found query search
                            inResultLkp++;
                        }
                    }
                    else if (!isHTML())
                    {
                        inResultLkp = 0;
                    }
                    
                    pos++;  // move to next char
                } // while
            }

        } // sequentialSearch



        static void Main(string[] args)
        {
            WebSearch webSearch = new WebSearch();
            // User input captures
            string input = null;
            while (input == null) {
                Console.WriteLine("Please enter the URL (or hit enter for lorem ipsom):");
                string candidate = Console.ReadLine();
                if (candidate.Length == 0)
                {
                    input = "http://digitaloversight.com/rsa-txt";
                }
                else
                {
                    input = webSearch.validateURL(candidate);
                }
            }
            Char[] searchInput = null;
            while (searchInput == null)
            {
                Console.WriteLine("Please enter the search query (or hit enter to search 'condimentum'):");
                string candidate = Console.ReadLine();
                if (candidate.Length == 0)
                {
                    searchInput = "condimentum".ToCharArray();
                }
                else
                {
                    searchInput = candidate.ToCharArray();
                }
            }
            webSearch.setQuery(searchInput);
            // Web Query creating
            WebRequest myWebRequest = WebRequest.Create( input );
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();

            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(ReceiveStream, encode);
            Console.WriteLine("\nResponse stream received");
            System.Threading.Thread.Sleep(2000);
            Char[] read = new Char[1024];

            // Read 1024 charcters at a time.     
            int count = readStream.Read(read, 0, 1024);
            
            while (count > 0)
            {
                webSearch.sequentialSearch(read);
                count = readStream.Read(read, 0, 1024); // Move to the next buffer on handle
            }
            // Validate graceful completion
            webSearch.validateGraceful();

            readStream.Close();
            myWebResponse.Close();
            System.Threading.Thread.Sleep(50000);

        }
    }
}
