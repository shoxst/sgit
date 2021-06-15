namespace sgit
{
  public static class Status
  {
    public static void Exec()
    {
      bool headIdxDiff = StatusChecker.CompareHeadAndIndex();
      bool idxWkdDiff = StatusChecker.CompareIndexAndWorkingDirectory();
      if (!headIdxDiff && !idxWkdDiff)
      {
        StatusChecker.PrintWorkingTreeClean();
        return;
      }
      if (headIdxDiff)
      {
        StatusChecker.PrintHeadAndIndexDiff();
      }
      if (idxWkdDiff)
      {
        StatusChecker.PrintIndexAndWorkingDirectoryDiff();
      }
    }
  }
}
