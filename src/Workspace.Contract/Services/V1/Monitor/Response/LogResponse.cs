﻿namespace Workspace.Contract
{
    public class LogResponse
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string FunctionType { get; set; }

        public string FunctionName { get; set; }

        public string Application {  get; set; }

        public DateTime CreatedDate { get; set; }

        public string BeforeValue { get; set; }

        public string AfterValue { get; set; }

        public Guid ObjId { get; set; }
    }
}
