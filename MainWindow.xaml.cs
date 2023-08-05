using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.IO;

namespace Encrypt_Decrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EncryptDecryptHashButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = InputTextBox.Text;
            string algorithmName = ((ComboBoxItem)AlgorithmComboBox.SelectedItem).Content.ToString();
            string passphrase = PassphraseTextBox.Text;

            if (EncryptRadioButton.IsChecked == true)
            {
                string encryptedText = EncryptText(inputText, algorithmName, passphrase);
                OutputTextBox.Text = encryptedText;
            }
            else if (DecryptRadioButton.IsChecked == true)
            {
                string decryptedText = DecryptText(inputText, algorithmName, passphrase);
                OutputTextBox.Text = decryptedText;
            }
            else if (HashRadioButton.IsChecked == true)
            {
                string hashedText = HashText(inputText, algorithmName);
                OutputTextBox.Text = hashedText;
            }
        }

        private string EncryptText(string text, string algorithmName, string passphrase)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(passphrase);
                aesAlg.IV = new byte[16]; // Initialization Vector
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = EncryptDecryptBytes(Encoding.UTF8.GetBytes(text), encryptor);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        private string DecryptText(string encryptedText, string algorithmName, string passphrase)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(passphrase);
                aesAlg.IV = new byte[16]; // Initialization Vector
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] decryptedBytes = EncryptDecryptBytes(encryptedBytes, decryptor);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        private string HashText(string text, string algorithmName)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(text);
                byte[] hash = sha256.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private byte[] EncryptDecryptBytes(byte[] data, ICryptoTransform transform)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                }
                return memoryStream.ToArray();
            }
        }
    }
}
