// --- Program Starts ---

    // The program will keep track of all shifts here.
    // When you close the program, this data might be lost unless you save it to a file.
    All_Recorded_Shifts = [] // (e.g., a list of records, where each record holds worker name, start, end, and hours)

    WHILE TRUE: // The program keeps running until you choose to exit
        DISPLAY "--- Worker Time Tracker Menu ---"
        DISPLAY "1. Record a New Work Shift"
        DISPLAY "2. View All Recorded Shifts"
        DISPLAY "3. Exit Program"
        DISPLAY "------------------------------"

        GET User_Choice:
            INPUT "Enter your choice (1, 2, or 3): "

        IF User_Choice IS "1":
            // Call the function to add a new shift
            CALL Function: RECORD_NEW_SHIFT()
        ELSE IF User_Choice IS "2":
            // Call the function to show all saved shifts
            CALL Function: VIEW_ALL_SHIFTS(All_Recorded_Shifts)
        ELSE IF User_Choice IS "3":
            DISPLAY "Exiting the program. Goodbye!"
            BREAK LOOP // Stop the program
        ELSE:
            DISPLAY "Invalid choice. Please enter 1, 2, or 3."

// --- Program Ends ---


// --- Function: RECORD_NEW_SHIFT ---
FUNCTION RECORD_NEW_SHIFT():
    DISPLAY "\n--- Record New Shift ---"
    GET Worker_Name:
        INPUT "Enter worker's name: "

    WHILE TRUE: // Loop until valid times are entered
        GET Start_DateTime_String:
            INPUT "Enter START date and time (YYYY-MM-DD HH:MM), e.g., 2025-06-09 08:00: "
            // The program will try to convert this text into a real date and time object.
            // If it fails (wrong format), it will tell you.

        GET End_DateTime_String:
            INPUT "Enter END date and time (YYYY-MM-DD HH:MM), e.g., 2025-06-09 17:00: "
            // Same here, convert to a date and time object.

        IF Conversion_Successful AND End_DateTime IS LATER_THAN Start_DateTime:
            BREAK LOOP // Valid times entered, exit this inner loop
        ELSE IF Conversion_Successful AND End_DateTime IS NOT_LATER_THAN Start_DateTime:
            DISPLAY "Error: End time cannot be the same as or before the start time. Please re-enter."
        ELSE: // Conversion_Failed
            DISPLAY "Error: Invalid date/time format. Please use YYYY-MM-DD HH:MM."

    CALCULATE Total_Working_Hours:
        // Subtract End_DateTime from Start_DateTime
        // Convert the difference (which is a duration) into total hours (e.g., 8.5 for 8 hours 30 minutes).

    CREATE New_Shift_Record:
        // Put all the collected information into one package
        New_Shift_Record = {
            "name": Worker_Name,
            "start": Start_DateTime_String, // Store as text, or as the actual date/time object
            "end": End_DateTime_String,     // Store as text, or as the actual date/time object
            "hours": Total_Working_Hours
        }

    ADD New_Shift_Record TO All_Recorded_Shifts

    DISPLAY "Shift recorded successfully for " + Worker_Name + " (" + FORMAT Total_Working_Hours + " hours)."
    DISPLAY "------------------------"


// --- Function: VIEW_ALL_SHIFTS ---
FUNCTION VIEW_ALL_SHIFTS(All_Recorded_Shifts):
    DISPLAY "\n--- All Recorded Work Shifts ---"
    IF All_Recorded_Shifts IS EMPTY:
        DISPLAY "No shifts have been recorded yet."
    ELSE:
        FOR EACH Shift_Record IN All_Recorded_Shifts:
            DISPLAY "Worker: " + Shift_Record.name
            DISPLAY "  Start: " + Shift_Record.start
            DISPLAY "  End:   " + Shift_Record.end
            DISPLAY "  Hours: " + Shift_Record.hours + " hours"
            DISPLAY "----------------------------"
import datetime
import csv
import os

# Define the CSV file where data will be stored
DATA_FILE = 'worker_timesheet.csv'
CSV_HEADERS = ['Worker Name', 'Start Time', 'End Time', 'Total Hours (decimal)']

def load_data():
    """Loads existing timesheet data from the CSV file."""
    data = []
    if os.path.exists(DATA_FILE):
        with open(DATA_FILE, mode='r', newline='', encoding='utf-8') as file:
            reader = csv.DictReader(file)
            if reader.fieldnames != CSV_HEADERS:
                print(f"Warning: CSV file '{DATA_FILE}' has unexpected headers. Please check the file.")
                print(f"Expected: {CSV_HEADERS}")
                print(f"Found:    {reader.fieldnames}")
                return [] # Return empty to prevent data corruption
            for row in reader:
                data.append(row)
    return data

def save_data(data):
    """Saves current timesheet data to the CSV file."""
    with open(DATA_FILE, mode='w', newline='', encoding='utf-8') as file:
        writer = csv.DictWriter(file, fieldnames=CSV_HEADERS)
        writer.writeheader()
        writer.writerows(data)
    print(f"Data saved to '{DATA_FILE}' successfully.")

def record_new_shift():
    """Prompts user for shift details, calculates hours, and adds to data."""
    print("\n--- Record New Work Shift ---")
    worker_name = input("Enter worker's name: ").strip()
    if not worker_name:
        print("Worker name cannot be empty. Returning to main menu.")
        return None

    while True:
        try:
            start_time_str = input("Enter START date and time (YYYY-MM-DD HH:MM), e.g., 2025-06-09 08:00: ").strip()
            start_datetime = datetime.datetime.strptime(start_time_str, "%Y-%m-%d %H:%M")

            end_time_str = input("Enter END date and time (YYYY-MM-DD HH:MM), e.g., 2025-06-09 17:00: ").strip()
            end_datetime = datetime.datetime.strptime(end_time_str, "%Y-%m-%d %H:%M")

            if end_datetime <= start_datetime:
                print("Error: End time must be after start time. Please re-enter.")
            else:
                break # Exit loop if times are valid
        except ValueError:
            print("Error: Invalid date/time format. Please use YYYY-MM-DD HH:MM.")

    time_difference = end_datetime - start_datetime
    total_hours = round(time_difference.total_seconds() / 3600, 2) # Calculate hours, round to 2 decimal places

    new_record = {
        'Worker Name': worker_name,
        'Start Time': start_datetime.strftime("%Y-%m-%d %H:%M"),
        'End Time': end_datetime.strftime("%Y-%m-%d %H:%M"),
        'Total Hours (decimal)': str(total_hours) # Store as string for CSV
    }
    print(f"Shift recorded: {worker_name} worked {total_hours} hours.")
    return new_record

def view_all_shifts(data):
    """Displays all recorded shifts in a formatted table."""
    print("\n--- All Recorded Work Shifts ---")
    if not data:
        print("No shifts have been recorded yet.")
        return

    # Determine column widths for nice formatting
    name_width = max(len(row['Worker Name']) for row in data) if data else len('Worker Name')
    name_width = max(name_width, len('Worker Name'))
    
    start_width = len('YYYY-MM-DD HH:MM')
    end_width = len('YYYY-MM-DD HH:MM')
    hours_width = max(len(row['Total Hours (decimal)']) for row in data) if data else len('Total Hours (decimal)')
    hours_width = max(hours_width, len('Hours')) # Ensure header fits
    
    # Print header
    print(f"{'Worker Name':<{name_width}} | {'Start Time':<{start_width}} | {'End Time':<{end_width}} | {'Hours':<{hours_width}}")
    print("-" * (name_width + start_width + end_width + hours_width + 9)) # +9 for separators

    # Print data rows
    for record in data:
        print(f"{record['Worker Name']:<{name_width}} | {record['Start Time']:<{start_width}} | {record['End Time']:<{end_width}} | {record['Total Hours (decimal)']:<{hours_width}}")
    print("--------------------------------")


def main():
    """Main function to run the timesheet program."""
    timesheet_data = load_data() # Load existing data at startup

    while True:
        print("\n--- Worker Timesheet Program ---")
        print("1. Record a New Work Shift")
        print("2. View All Recorded Shifts")
        print("3. Exit Program")
        print("------------------------------")

        choice = input("Enter your choice (1, 2, or 3): ").strip()

        if choice == '1':
            new_shift = record_new_shift()
            if new_shift:
                timesheet_data.append(new_shift)
                save_data(timesheet_data) # Save immediately after adding
        elif choice == '2':
            view_all_shifts(timesheet_data)
        elif choice == '3':
            print("Exiting the program. Goodbye!")
            break
        else:
            print("Invalid choice. Please enter 1, 2, or 3.")

if __name__ == "__main__":
    main()
