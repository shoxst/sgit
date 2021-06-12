using System;
using System.Collections.Generic;

namespace sgit
{
  public class TreeObject : SgitObject
  {
    public TreeObject() : base(ObjectType.tree)
    {
    }
  }
}
