using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using TMPro;
using UnityEngine.UI;

public class ShowContacts : MonoBehaviour
{
    public GameObject contactPrefab;
    public Transform contantContainer;
    AddressBookContactsAccessStatus status;
    // Start is called before the first frame update
    void Start()
    {
        // Request permission to read contacts when the game starts
        status = AddressBook.GetContactsAccessStatus();
        ReadContacts();
    }

    public void ReadContacts()
    {
        Debug.Log("Contacts access status: " + status);
        if (status == AddressBookContactsAccessStatus.NotDetermined)
        {
            Debug.Log("Requesting contacts access...");
            AddressBook.RequestContactsAccess(callback: OnRequestContactsAccessFinish);
        }
        else if (status == AddressBookContactsAccessStatus.Authorized)
        {
            Debug.Log("Contacts access already authorized. Reading contacts...");
            AddressBook.ReadContacts(OnReadContactsFinish);
        }
        else
        {
            Debug.Log("Contacts access denied or restricted.");
        }
    }
    //callback function to check user's action on request
    private void OnRequestContactsAccessFinish(AddressBookRequestContactsAccessResult result, Error error)
    {
        Debug.Log("Request for contacts access finished.");
        Debug.Log("Address book contacts access status: " + result.AccessStatus);
        if (result.AccessStatus == AddressBookContactsAccessStatus.Authorized)
        {
            AddressBook.ReadContacts(OnReadContactsFinish);
        }
        else
        {
            Debug.Log("Permission denied");
        }
    }
    // Callback function for when contacts reading 
    private void OnReadContactsFinish(AddressBookReadContactsResult result, Error error)
    {
        if (error == null)
        {
            // Get the contacts from the result
            var contacts = result.Contacts;

            // Log information about the contacts
            Debug.Log("Request to read contacts finished successfully.");
            Debug.Log("Total contacts fetched: " + contacts.Length);
            Debug.Log("Below are the contact details (capped to first 10 results only):");

            // Iterate through the contacts (up to 10 for simplicity)
            for (int iter = 0; iter < contacts.Length && iter < 10; iter++)
            {
                // Log contact information
                //Debug.Log(contacts[iter]);

                // Instantiate the contactPrefab
                GameObject instantiatedContact = Instantiate(contactPrefab);
                instantiatedContact.transform.SetParent(contantContainer);

                // Access the TextMeshProUGUI components in the instantiatedContact
                TextMeshProUGUI[] textComponents = instantiatedContact.GetComponentsInChildren<TextMeshProUGUI>();

                // Assuming the first TextMeshProUGUI is for the first name, and the second is for the last name
                TextMeshProUGUI firstNameText = textComponents[0];
                TextMeshProUGUI lastNameText = textComponents[1];

                // Set the names in the TextMeshProUGUI components
                firstNameText.text = contacts[iter].FirstName;
                lastNameText.text = contacts[iter].LastName;

                // Add a button to send an email
                Button emailButton = instantiatedContact.GetComponentInChildren<Button>();

                // Check if there are any email addresses available in the array
                bool hasEmailAddresses = contacts[iter].EmailAddresses != null && contacts[iter].EmailAddresses.Length > 0;

                // Check if mail can be sent using built-in function and there are email addresses
                if (MailComposer.CanSendMail() && hasEmailAddresses)
                {
                    // Enable the button
                    emailButton.interactable = true;

                    // Add a listener to the button to trigger email composition
                    emailButton.onClick.AddListener(() => SendEmail(contacts[iter].EmailAddresses));
                }
                else
                {
                    // Log if no email addresses found or mail cannot be sent
                    //Debug.Log("No email found or unable to send email.");

                    // Disable the button if mail cannot be sent
                    emailButton.interactable = false;
                }
            }
            
        }
        else
        {
            // Log if reading contacts failed with an error
            Debug.Log("Request to read contacts failed with error. Error: " + error);
        }
    }

    // Function to send an email with given email addresses
    private void SendEmail(string[] emailAddresses)
    {
        if (emailAddresses != null && emailAddresses.Length > 0)
        {
            // Iterate through the array of email addresses
            foreach (string emailAddress in emailAddresses)
            {
                // Use Mail Composer to send an email
                MailComposer composer = MailComposer.CreateInstance();
                composer.SetToRecipients(new string[] { emailAddress });
                composer.SetSubject("Subject of the email");
                composer.SetBody("Body of the email", false); // Pass true if string is html content
                composer.SetCompletionCallback((result, error) => {
                    // Log the result code when mail composer is closed
                    Debug.Log("Mail composer was closed. Result code: " + result.ResultCode);
                });
                composer.Show();
            }
        }
        else
        {
            // Log a warning if no email addresses are available for the contact
            Debug.LogWarning("No email addresses available for the contact.");
        }
    }



}
