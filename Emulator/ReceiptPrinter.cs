﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ReceiptPrinterEmulator.EscPos;
using ReceiptPrinterEmulator.Logging;

namespace ReceiptPrinterEmulator.Emulator;

public class ReceiptPrinter
{
    private readonly EscPosInterpreter _escPosInterpreter;

    public Receipt CurrentReceipt { get; private set; }
    public List<Receipt> ReceiptStack { get; private set; }

    public event EventHandler<EventArgs> OnActivityEvent; 

    public ReceiptPrinter()
    {
        _escPosInterpreter = new(this);

        ReceiptStack = new();

        StartNewReceipt();
    }

    #region ESC/POS
    public void FeedEscPos(string ascii)
    {
        File.WriteAllText("last_escpos_receive.txt", ascii, Encoding.ASCII);

        try
        {
            _escPosInterpreter.Interpret(ascii);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, "ESC/POS Interpreter Error");
        }

        OnActivityEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Receipt meta
    private void StartNewReceipt()
    {
        CurrentReceipt = new();
        ReceiptStack.Add(CurrentReceipt);
    }
    #endregion

    #region Direct API
    public void PrintText(string text)
    {
        
    }
    
    public void Cut()
    {
        StartNewReceipt();
    }

    /// <summary>
    /// Feeds one line, based on the current line spacing.
    /// </summary>
    /// <remarks>
    /// - The amount of paper fed per line is based on the value set using the line spacing command (ESC 2 or ESC 3).
    /// </remarks>
    public void LineFeed()
    {
        
    }
    #endregion

    #region Command API
    /// <summary>
    /// Prints the data in the print buffer and feeds one line, based on the current line spacing.
    /// </summary>
    public void PrintAndLineFeed(string printBuffer)
    {
        PrintText(printBuffer);
        LineFeed();
    }
    #endregion
}