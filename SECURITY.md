# Security Policy

## Supported Versions

Use this section to tell people about which versions of your project are
currently being supported with security updates.

| Version | Supported          |
| ------- | ------------------ |
| 5.1.x   | :white_check_mark: |
| 5.0.x   | :x:                |
| 4.0.x   | :white_check_mark: |
| < 4.0   | :x:                |

## Reporting a Vulnerability

Use this section to tell people how to report a vulnerability.

Tell them where to go, how often they can expect to get an update on a
reported vulnerability, what to expect if the vulnerability is accepted or
declined, etc.
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
