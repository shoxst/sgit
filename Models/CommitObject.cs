using System;
using System.Collections.Generic;

namespace sgit
{
  public class CommitObject : SgitObject
  {
    public CommitObject() : base(ObjectType.commit)
    {
    }
  }
}
