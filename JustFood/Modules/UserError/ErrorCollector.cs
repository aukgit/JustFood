using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JustFood.Modules.UserError {
    public class ErrorCollector {
        readonly List<BasicError> errors;
        short orderIncrementer = 0;
        public enum ErrorType {
            High,
            Medium,
            Low
        }

        int defaultCapacity = 60;
        public class BasicError {
            [Required]
            public short OrderID { get; set; }
            [Required]
            public string Message { get; set; }
            [Required]
            public ErrorType Type { get; set; }
        }

        /// <summary>
        /// Is any error exist in the list?
        /// </summary>
        /// <returns>Returns true if any error exist.</returns>
        public bool IsExist() {
            if (errors != null && errors.Count > 0) {
                return true;
            }
            return false;
        }

        public ErrorCollector(int def = 60) {
            errors = new List<BasicError>(def);
            defaultCapacity = def;
        }

        /// <summary>
        /// add error message with low priority
        /// </summary>
        /// <param name="msg">set your message.</param>
        /// <param name="quantityTypeIsNotValidPleaseSelectAValidQuantityType"></param>
        public int Add(string msg, string quantityTypeIsNotValidPleaseSelectAValidQuantityType) {
            var error = new BasicError() {
                OrderID = orderIncrementer++,
                Message = msg,
                Type = ErrorType.Low
            };
            errors.Add(error);
            return error.OrderID;
        }

        /// <summary>
        /// add error message with high priority
        /// </summary>
        /// <param name="msg">set your message.</param>
        public int AddHigh(string msg) {
            var error = new BasicError() {
                OrderID = orderIncrementer++,
                Message = msg,
                Type = ErrorType.High
            };
            errors.Add(error);
            return error.OrderID;
        }

        /// <summary>
        /// add error message with medium priority
        /// </summary>
        /// <param name="msg">set your message.</param>
        public int AddMedium(string msg) {
            var error = new BasicError() {
                OrderID = orderIncrementer++,
                Message = msg,
                Type = ErrorType.Medium
            };
            errors.Add(error);
            return error.OrderID;
        }

        /// <summary>
        /// add error message with low priority
        /// </summary>
        /// <param name="msg">set your message.</param>
        /// <param name="type">Type of your error message.</param>
        public int Add(string msg) {
            var error = new BasicError() {
                OrderID = orderIncrementer++,
                Message = msg,
                Type = ErrorType.Low
            };
            errors.Add(error);
            return error.OrderID;
        }
        /// <summary>
        /// add error message with given priority
        /// </summary>
        /// <param name="msg">set your message.</param>
        /// <param name="type">Type of your error message.</param>
        public int Add(string msg, ErrorType type) {
            var error = new BasicError() {
                OrderID = orderIncrementer++,
                Message = msg,
                Type = type
            };
            errors.Add(error);
            return error.OrderID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns all error message as string list.</returns>
        public List<string> GetMessages() {
            return errors.Select(n => n.Message)
                         .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns all error message as Error Object.</returns>
        public List<BasicError> GetErrors() {
            return errors.ToList();
        }

        public void Remove(int OrderID) {
            var error = errors.FirstOrDefault(n => n.OrderID == OrderID);
            if (error != null) {
                errors.Remove(error);
            }
        }

        public void Remove(string Message) {
            var error = errors.FirstOrDefault(n => n.Message == Message);
            if (error != null) {
                errors.Remove(error);
            }
        }

        /// <summary>
        /// Clean counter and clean the error list start from 0.
        /// </summary>
        public void Clear() {
            orderIncrementer = 0;
            errors.Clear();
            //errors.Capacity = defaultCapacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns high error message as string list.</returns>
        public List<string> GetMessagesHigh() {
            return errors.Where(n=> n.Type == ErrorType.High).Select(n => n.Message).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns low error message as string list.</returns>
        public List<string> GetMessagesLow() {
            return errors.Where(n => n.Type == ErrorType.Low).Select(n => n.Message).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns medium error message as string list.</returns>
        public List<string> GetMessagesMedium() {
            return errors.Where(n => n.Type == ErrorType.Medium).Select(n => n.Message).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns all error message as string list of sorted by order id.</returns>
        public List<string> GetMessagesSorted() {
            return errors.OrderBy(n=> n.OrderID).Select(n=> n.Message).ToList();
        }

    }
}