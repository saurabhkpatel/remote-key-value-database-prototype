using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2Starter
{
    public class QueryEngine<Key, Value>
    {
        private DBEngine<Key, DBElement<Key, Value>> db = null;
        private DBFactory<Key, DBElement<Key, Value>> dbFactory = new DBFactory<Key, DBElement<Key, Value>>();

        public QueryEngine(DBEngine<Key, DBElement<Key, Value>> dbEngine)
        {
            db = dbEngine;
        }

        public Func<Key, bool> defineTimeStampQuery(DateTime startTime, DateTime? endTime = null)
        {
            Func<Key, bool> queryPredicate = (Key key) =>
            {
                if (!db.Keys().Contains(key))
                    return false;
                else
                {
                    if (endTime == null)
                        endTime = DateTime.Now;
                    DBElement<Key, Value> value;
                    db.getValue(key, out value);
                    DBElement<Key, Value> elem = value as DBElement<Key, Value>;
                    int cond1 = DateTime.Compare(elem.timeStamp, startTime);
                    int cond2 = DateTime.Compare(elem.timeStamp, (DateTime)endTime);
                    if (cond1 >= 0 && cond2 <= 0)
                    {
                        return true;
                    }
                }
                return false;
            };
            return queryPredicate;
        }

        public Func<Key, bool> defineQueryKeyPatternSearch(string queryTerm)
        {
            Func<Key, bool> queryPredicate = null;
            queryPredicate = (Key key) =>
            {
                if (db != null && key.ToString().Contains(queryTerm))
                    return true;
                return false;
            };
            return queryPredicate;
        }

        public Func<Key, bool> defineQueryValuePatternSearch(string search)
        {
            Func<Key, bool> queryPredicate = null;
            queryPredicate = (Key key) =>
            {
                if (db != null)
                {
                    if (db.Dictionary[key].name.Contains(search))
                        return true;
                    else if (db.Dictionary[key].descr.Contains(search))
                        return true;
                    else if (db.Dictionary[key].timeStamp.ToString().Contains(search))
                        return true;
                    else if (db.Dictionary[key].payload != null)
                    {
                        ListOfStrings payload = db.Dictionary[key].payload as ListOfStrings;
                        foreach (var item in payload.theWrappedData)
                        {
                            if (item.ToString().Contains(search))
                                return true;
                        }
                    }
                    else if (db.Dictionary[key].children != null)
                    {
                        List<Key> childrens = db.Dictionary[key].children;
                        foreach (var item in childrens)
                        {
                            if (item.ToString().Contains(search))
                                return true;
                        }
                    }

                }
                return false;
            };
            return queryPredicate;

        }

        public void processValueQuery(Key key, out DBElement<Key, Value> value)
        {
            value = default(DBElement<Key, Value>);
            if (db != null && db.Dictionary.Keys.Contains(key))
            {
                db.getValue(key, out value);
            }
        }

        public bool processChildrenQuery(Key key, out List<Key> childrens)
        {
            childrens = new List<Key>();
            if (db != null && db.Dictionary.Keys.Contains(key))
            {
                childrens = db.Dictionary[key].children;
                if (childrens.Count() != 0)
                    return true;
            }
            return false;
        }

        public bool processPatternMatchInKeysQuery(Func<Key, bool> queryPredicate)
        {
            IEnumerable<Key> keysCollection = db.Keys();
            foreach (var item in keysCollection)
            {
                if (queryPredicate(item))
                {
                    dbFactory.DBEngine.insert(item, db.Dictionary[item]);
                }
            }
            if (dbFactory.DBEngine.Dictionary.Keys.Count() == 0)
                return false;
            else
                return true;
        }

        public bool processPatternMatchInMetaDataQuery(Func<Key, bool> queryPredicate)
        {
            IEnumerable<Key> keysCollection = db.Keys();
            foreach (var item in keysCollection)
            {
                if (queryPredicate(item))
                {
                    dbFactory.DBEngine.insert(item, db.Dictionary[item]);
                }
            }
            if (dbFactory.DBEngine.Dictionary.Keys.Count() == 0)
                return false;
            else
                return true;
        }

        public bool processCompoundQuery(Func<Key, bool> queryPredicate)
        {
            IEnumerable<Key> keysCollection = dbFactory.DBEngine.Keys();
            dbFactory.DBEngine.Dictionary.Clear();
            foreach (var item in keysCollection)
            {
                if (queryPredicate(item))
                {
                    dbFactory.DBEngine.insert(item, db.Dictionary[item]);
                }
            }
            if (dbFactory.DBEngine.Dictionary.Keys.Count() == 0)
                return false;
            else
                return true;
        }

    }
}
