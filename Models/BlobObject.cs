using System;

namespace sgit
{
  public class BlobObject : SgitObject
  {
    public BlobObject() : base(ObjectType.blob)
    {
    }
  }
}
