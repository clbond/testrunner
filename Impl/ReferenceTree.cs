﻿using System.Collections.Generic;

namespace ContinuousRunner.Impl
{
    public class ReferenceTree : IReferenceTree
    {
        #region Implementation of IReferenceTree
        
        public IEnumerable<IScript> GetDependents(IScript script)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IScript> GetDependencies(IScript script)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}
