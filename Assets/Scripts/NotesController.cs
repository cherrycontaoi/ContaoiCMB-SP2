using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NotesController : MonoBehaviour
{

    // [SerializeField] private GameObject notesPanel;
    [SerializeField] private Transform container;
    [SerializeField] private Note notePrefab;
    [SerializeField] private List<NoteText> notes = new List<NoteText>();

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
            var newNote = Instantiate(notePrefab, container);
            newNote.SetNote(note.Text);
        }
    }
    private void ClearNotes()
    {
        foreach(Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }


    // public List<string> Notes;

    // // void AddNote(TMP_Text note)
    // // {
    // //     Notes.Append(note);
    // // }

    // public TextMeshProUGUI output;
    public TMP_InputField textInput;

    public void AddNoteButton()
    {
        NoteText newNote = new NoteText
        {
            Text = "- " + textInput.text
        };
        AddNote(newNote);
        notes.Add(newNote);
        textInput.text = "";
        // CreateNotes();
    }

    private void AddNote(NoteText newNoteText)
    {
        var newNote = Instantiate(notePrefab, container);
        newNote.SetNote(newNoteText.Text);
    }




}
