using Microsoft.Extensions.Configuration;
using SwDeveloperExercise.Constants;
using SwDeveloperExercise.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SwDeveloperExercise
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Bind to strongly-typed class
            var appSettings = config.GetSection("AppSettings").Get<AppSettings>();

            string strReadFile = $"{appSettings.FilePath}sales.txt";
            var fileLines = File.ReadAllLines(strReadFile).ToList();
            List<SalesItem> sales = new List<SalesItem>();
            try
            {
                Program p = new Program();
                string dateFormat = p.ReadDateFormat();

                if (string.IsNullOrEmpty(dateFormat))
                {
                    Console.WriteLine("Invalid date format selected");
                }
                else
                {
                    foreach (string line in fileLines)
                    {
                        SalesItem sale = new SalesItem();
                        sale.Date = DateTime.ParseExact(line.Split(new string[] { appSettings.Delimiter }, StringSplitOptions.None)[0], p.ConvertDateFormat(dateFormat), CultureInfo.InvariantCulture);
                        sale.Amount = Decimal.Parse(line.Split(new string[] { appSettings.Delimiter }, StringSplitOptions.None)[1]);
                        sales.Add(sale);
                    }

                    Console.WriteLine("Enter from year for Average:");
                    string fromYearAvg = Console.ReadLine();
                    Console.WriteLine("Enter to year for Average:");
                    string toYearAvg = Console.ReadLine();

                    p.AverageCalculation(sales, fromYearAvg, toYearAvg);

                    Console.WriteLine("Enter specific year for Standard Deviation:");
                    string specYear = Console.ReadLine();

                    p.StandardDeviationCalculation(sales, specYear);

                    Console.WriteLine("Enter from year for Standard Deviation:");
                    string fromYearSD = Console.ReadLine();
                    Console.WriteLine("Enter to year for Standard Deviation:");
                    string toYearSD = Console.ReadLine();

                    p.StandardDeviationCalculation(sales, fromYearSD, toYearSD);

                    Console.ReadKey();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.ReadKey();
            }
        }

        #region Private Mathods
        private string ReadDateFormat()
        {
            Console.WriteLine("Enter date format:");
            Console.WriteLine("1. dd/MM/yyyy");
            Console.WriteLine("2. ddMMyyyy");
            Console.WriteLine("3. dd-MM-yyyy");
            Console.WriteLine("4. yyyy/MM/dd");
            Console.WriteLine("5. yyyyMMdd");
            Console.WriteLine("6. yyyy-MM-dd");

            var dateFormat = Console.ReadLine();

            if (int.TryParse(dateFormat, out int parsedValue))
            {
                while (parsedValue > 6 || parsedValue < 1)
                {
                    Console.WriteLine("Invalid selection");
                }

                return dateFormat;
            }
            else
            {
                Console.WriteLine("You have to select from 1 to 6");
                return null;
            }
        }

        private string ConvertDateFormat(string selectedFormat)
        {
            if (selectedFormat == "1")
            {
                return DateTimeHelper.ddMMyyyy_slash;
            }
            else if (selectedFormat == "2")
            {
                return DateTimeHelper.ddMMyyyy;
            }
            else if (selectedFormat == "3")
            {
                return DateTimeHelper.dd_MM_yyyy;
            }
            else if (selectedFormat == "4")
            {
                return DateTimeHelper.yyyyMMdd_slash;
            }
            else if (selectedFormat == "5")
            {
                return DateTimeHelper.yyyyMMdd;
            }
            else
            {
                return DateTimeHelper.yyyy_MM_dd;
            }
        }

        private void AverageCalculation(List<SalesItem> sales, string fromYear, string toYear)
        {
            decimal avg = sales.Where(x => x.Date.Year >= int.Parse(fromYear) && x.Date.Year <= int.Parse(toYear))
                .Select(x => x.Amount).DefaultIfEmpty(0).Average();
            Console.WriteLine($"Average ({fromYear}-{toYear}): {avg}");
        }

        private void StandardDeviationCalculation(List<SalesItem> sales, string year)
        {
            IEnumerable<double> amounts = sales.Where(x => x.Date.Year == int.Parse(year))
                .Select(x => (double)x.Amount).DefaultIfEmpty(0);
            double avg = amounts.Average();

            double sumOfSquaresOfDifferences = amounts.Select(val => (val - avg) * (val - avg)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / amounts.Count());
            Console.WriteLine($"Standard Deviation ({year}): {sd}");
        }

        private void StandardDeviationCalculation(List<SalesItem> sales, string fromYear, string toYear)
        {
            IEnumerable<double> amounts = sales.Where(x => x.Date.Year >= int.Parse(fromYear) && x.Date.Year <= int.Parse(toYear))
                .Select(x => (double)x.Amount).DefaultIfEmpty(0);
            double avg = amounts.Average();

            double sumOfSquaresOfDifferences = amounts.Select(val => (val - avg) * (val - avg)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / amounts.Count());
            Console.WriteLine($"Standard Deviation ({fromYear}-{toYear}): {sd}");
        }

        #endregion Private Mathods
    }
}
