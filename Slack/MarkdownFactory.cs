using System;
using System.Collections.Generic;
using System.Linq;
using BottleCapApi.Models;

namespace BottleCapApi.Slack
{
  public class MarkdownFactory
  {
    private static string LINE_BREAK = "\n";
    private static string _Divider(char ch = '-', int length = 10)
    {
      return new String(ch, length);
    }


    private static string _ColumnValue(string value, int minLength, bool includeBar = true)
    {
      return " " + $"{value}".PadRight(minLength - 1) + $"{(includeBar ? "|" : "")}";
    }

    public static string CreateTable(IEnumerable<Player> players)
    {
      var playerColumnWidth = 20;
      var capColumnWidth = 7;
      var playerData = String.Join("", players.Select(s => $"{_ColumnValue(s.BottleCaps.ToString(), capColumnWidth)}{_ColumnValue(s.SlackId, playerColumnWidth, false)}{LINE_BREAK}"));

      var emptyData = $"{_ColumnValue("None yet....", playerColumnWidth, false)}{LINE_BREAK}"; ;
      var rv = $"```{_Divider('=', capColumnWidth)}{_Divider('=', playerColumnWidth)} {LINE_BREAK}"
             + $"{_ColumnValue("Caps", capColumnWidth)}{_ColumnValue("Players", playerColumnWidth, false)}{LINE_BREAK}"
             + $"{_Divider('-', capColumnWidth)}{_Divider('-', playerColumnWidth)}{LINE_BREAK}"
             + $"{(players == null || players.Count() == 0 ? emptyData : playerData)}"
             + $"{_Divider('=', capColumnWidth)}{_Divider('=', playerColumnWidth)} {LINE_BREAK}```";
      return rv;
    }
    public static string CreateCards(IEnumerable<Player> players)
    {
      var playerColumnWidth = 30;
      var capColumnWidth = 7;
      var playerData = String.Join("", players.Select(s => $"|{_ColumnValue(s.SlackId, playerColumnWidth)}{_ColumnValue(s.BottleCaps.ToString(), capColumnWidth)}{LINE_BREAK}"));

      var emptyData = $"|{_ColumnValue("None yet....", playerColumnWidth)}{_ColumnValue("", capColumnWidth)}{LINE_BREAK}"; ;
      var rv = $"```"
             + $"|{_ColumnValue("Players", playerColumnWidth)}{_ColumnValue("Caps", capColumnWidth)}{LINE_BREAK}"
             + $"{_Divider('-', playerColumnWidth)} {_Divider('-', capColumnWidth)}{LINE_BREAK}"
             + $"{(players == null || players.Count() == 0 ? emptyData : playerData)}"
             + $"{_Divider('=', playerColumnWidth)} {_Divider('=', capColumnWidth + 2)}{LINE_BREAK}```";
      return rv;
    }
  }
}