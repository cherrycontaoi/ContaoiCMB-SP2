using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NotesController : MonoBehaviour
{

    // [SerializeField] private GameObject notesPanel;
    [SerializeField] private ToggleSystem toggleSystem;
    [SerializeField] private Transform notesContainer;
    [SerializeField] private Transform ordersContainer;
    [SerializeField] private Note notePrefab;
    [SerializeField] private Order orderPrefab;
    [SerializeField] private List<NoteText> notes = new List<NoteText>();
    [SerializeField] private List<NoteText> orders = new List<NoteText>();
    public TMP_InputField textInput;

    public class NoteText
    {
        public string Text;
    }

    private void Start()
    {
        CreateNotes();
    }

    private void CreateNotes()
    {
        ClearNotes();

        foreach(var note in notes)
        {
            var newNote = Instantiate(notePrefab, notesContainer);
            newNote.SetNote(note.Text);
        }

        foreach(var order in orders)
        {
            var newOrder = Instantiate(orderPrefab, ordersContainer);
            newOrder.SetOrder(order.Text);
        }
    }
    private void ClearNotes()
    {
        foreach(Transform child in notesContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(Transform child in ordersContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddNoteButton()
    {
        string noteType = toggleSystem.getToggledOn();
        NoteText newNote = new NoteText
        {
            Text = textInput.text
            //  Text = "- " + textInput.text
        };

        if (noteType == "Note")
        {
            AddNote(newNote);
            notes.Add(newNote);
        }

        else if (noteType == "Order")
        {
            AddOrder(newNote);
            orders.Add(newNote);
        }
        
        textInput.text = "";
        
    }

    private void AddNote(NoteText newNoteText)
    {
        var newNote = Instantiate(notePrefab, notesContainer);
        newNote.SetNote(newNoteText.Text);
    }

    private void AddOrder(NoteText newNoteText)
    {
        var newNote = Instantiate(orderPrefab, ordersContainer);
        newNote.SetOrder(newNoteText.Text);
    }







}
