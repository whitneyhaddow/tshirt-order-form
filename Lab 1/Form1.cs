using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lab_1
/***************************************************
* This form calculates total price for an order
* based on t-shirt style, size, and quantity.
* Created November 2014 by Whitney Haddow
 * Used the following webpage for help splitting data file into 2D array:
 * http://stackoverflow.com/questions/9674469/create-2d-array-from-txt-file
****************************************************/
{
    public partial class Form1 : Form
    {
        //declare the arrays
        string[] sizes = { "S", "M", "L", "XL" };
        int countSizes; // how many sizes
        string[] styles = { "Plain Black/White", "Plain Colour", "White w/Black Logo", "Colour w/Black Logo", "Black w/Sliver Logo", "Colour w/Silver Logo" };
        int countStyles; // how many styles
        decimal[,] prices; // two dimensional array of prices

        //declare constants
        const decimal TAX_PERCENT = 0.05m;

        //global variables
        decimal unitPrice;

       
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // find number of styles and sizes
            countSizes = sizes.Length;
            countStyles = styles.Length;
                   
            // create two dimensional array
            prices = new decimal[countStyles, countSizes];

            //read the prices file, store in array, display in listbox
            LoadData();

            //load the style types into cboStyle - plain black/white selected
            for (int i = 0; i < countStyles; i++)
            {
                cboStyle.Items.Add(styles[i]);
            }
            cboStyle.SelectedIndex = 0;

            //load the sizes into cboSize - small selected
            for (int j = 0; j < countSizes; j++)
            {
                cboSize.Items.Add(sizes[j]);
            }
            cboSize.SelectedIndex = 0;

            //populate unit price
            unitPrice = ReturnUnitPrice();
            txtUnitPrice.Text = String.Format("{0:c}", unitPrice);
            
        }

        //change the unit price when style and size are selected
        private void cboSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            unitPrice = ReturnUnitPrice();
            txtUnitPrice.Text = String.Format("{0:c}", unitPrice);
        }
        
        private bool LoadData()
        {
            string path = "ABC_TEXTILES_PRICES.txt";
            FileStream fs;
            StreamReader sr; // for file reading
            try
            {
                // open the file
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("The file was not found.");
                return false;
            }

            // file opened successfully
            // read file and store data into the array
            try
            {
                while (!sr.EndOfStream)
                {
                    string line; // one line from the file
                    string[] parts; // line will be split into parts
                    
                    
                    int x, s; // indexes for sizes and styles respectively
                    lstShirts.Items.Clear(); // start with an empty list box
                    string header = "Styles/Sizes";
                    for (x = 0; x < countSizes; x++) //puts sizes across the top
                        header += " \t\t " + sizes[x];
                    lstShirts.Items.Add(header);
                    lstShirts.Items.Add("----------------------------------------------------------------------------------------");
                    string rows;
                    for (s = 0; s < countStyles; s++) // for each row
                    {
                        line = sr.ReadLine(); // read next line
                        parts = line.Split(','); // split the line into parts using comma as delimiter
                        rows = styles[s] + "\t";
                        if (styles[s].Length < 18) rows += "\t"; //for short style names add extra tab
                        for (x = 0; x < countSizes; x++)
                        {                          
                            prices[s, x] = decimal.Parse(parts[x]);  
                            rows += prices[s, x] + "\t\t"; // prices for given style and size
                        }
                        lstShirts.Items.Add(rows);
                    }
                }

                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("I/O error occurred while reading from the file: " +
                            ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unanticipated error occurred while reading from the  file: " +
                            ex.Message);
                return false;
            }
            finally
            {
                sr.Close(); // close the file
            }
        } // end LoadData

       //method to determine the value of unitPrice
        public decimal ReturnUnitPrice()
        {
            decimal unitPrice; //result
            int cboStyleIndex = cboStyle.SelectedIndex;
            int cboSizeIndex = cboSize.SelectedIndex;
            unitPrice = prices[cboStyleIndex, cboSizeIndex];
            return unitPrice;
        }


        private void btnCalculate_Click(object sender, EventArgs e)
        {
            //read the inputs (cbo boxes and order quantity}
            decimal quantity;
            quantity = Convert.ToDecimal(txtQuantity.Text);
            unitPrice = ReturnUnitPrice();
                      
            //calculate totals
            if (IsNonNegativeDecimal(txtQuantity.Text))
            {
                decimal subtotal = unitPrice * quantity;
                decimal tax = subtotal * TAX_PERCENT;
                decimal total = subtotal + tax;


                //display subtotal, tax, and total into the labels
                lblSubtotal.Text = String.Format("{0:c}", subtotal);
                lblTax.Text = String.Format("{0:c}", tax);
                lblTotal.Text = String.Format("{0:c}", total);
            }
        }

       
        //validate order quantity
        public bool IsNonNegativeDecimal(string input)
        {
            bool result = true; //default
            if (input == "")
            {
                MessageBox.Show("Please enter order quantity.",
                   "Error");
                return false;
            }
            else //not empty
            {
                decimal val; //temporary value to tryparse
                if (!decimal.TryParse(input, out val))
                {
                    MessageBox.Show("Order quantity must be entered as a number.",
                   "Error");
                    return false;
                }
                else
                {
                    if (val < 0)
                    {
                        MessageBox.Show("Order quantity must be entered as number above 0.",
                       "Error");
                        return false;
                    }
                    else
                        return true;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //clear all fields and reset cbo boxes
            txtQuantity.Text = "";
            lblSubtotal.Text = "";
            lblTax.Text = "";
            lblTotal.Text = "";
            cboStyle.SelectedIndex = 0;
            cboSize.SelectedIndex = 0;

        }

        //terminate application
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

             
     }

}
