Imports System.Text
Imports System.Text.RegularExpressions

Public Class Form1
    Private total As Integer = 0 'total price for appointment
    Private pendingAppointmentDetails As String = "" ' Store appointment details until payment is complete

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadAllData()
        generateNextIDs()
        loadPlaceholders()
        updateVaccineStatusAndNextVisit()
    End Sub

    ' ========== REUSABLE METHODS ==========
    Private Sub saveToFile(filePath As String, data As String)
        'called to save data to a specified file
        IO.File.AppendAllText(filePath, data & Environment.NewLine)
    End Sub

    Private Function readFromFile(filePath As String) As String()
        'called to read data from a specific database file (appointments, pets, owners)
        If IO.File.Exists(filePath) Then
            Return IO.File.ReadAllLines(filePath)
        End If
        Return New String() {}
    End Function

    Private Function calculatePrice(petType As String, procedure As String) As Integer
        'local variables to hold prices based on pet type
        Dim checkUpPrice As Integer = 0
        Dim vaccinePrice As Integer = 0

        Select Case petType
            Case "Canine"
                checkUpPrice = 400
                vaccinePrice = 1500
            Case "Feline"
                checkUpPrice = 500
                vaccinePrice = 1000
            Case "Reptile"
                checkUpPrice = 600
                vaccinePrice = 2000
        End Select

        Select Case procedure
            Case "Checkup" : Return checkUpPrice
            Case "Vaccine" : Return vaccinePrice
            Case "Vaccine-Checkup" : Return checkUpPrice + vaccinePrice
            Case Else : Return 0
        End Select
    End Function

    Private Function getPetType(ownerName As String, petName As String) As String
        'called to validate pet type based on owner name and pet name if they exist in the pet table
        If String.IsNullOrEmpty(ownerName) OrElse String.IsNullOrEmpty(petName) Then
            Return "N/A"
        End If

        'if first condition fails, search pet table for matching owner name and pet name to get pet type
        For Each petRow As DataGridViewRow In tblPets.Rows
            If petRow.Cells(5).Value IsNot Nothing AndAlso petRow.Cells(5).Value.ToString().Trim() = ownerName.Trim() AndAlso
               petRow.Cells(1).Value IsNot Nothing AndAlso petRow.Cells(1).Value.ToString().Trim() = petName.Trim() Then
                Return If(petRow.Cells(2).Value?.ToString(), "N/A")
            End If
        Next

        'returns N/A if no match is found
        Return "N/A"
    End Function

    Private Function getPetDetailsFromFile(petID As String) As String()
        'called to get full pet details from the pet database file based on pet ID (used for displaying pet details)
        Dim petFilePath As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_Pets.txt")
        If IO.File.Exists(petFilePath) Then
            Dim allPets As String() = IO.File.ReadAllLines(petFilePath)
            For Each petLine As String In allPets
                Dim fields As String() = petLine.Split("|"c)
                If fields.Length >= 9 AndAlso fields(0) = petID Then
                    Return fields
                End If
            Next
        End If
        Return Nothing
    End Function

    '========== PLACEHOLDER METHODS ==========
    Private Sub loadPlaceholders()
        'help user to what to input in fields
        SetupPlaceholder(txtOwnerName, "Enter owner's name")
        SetupPlaceholder(txtPetName, "Enter pet's name")
        SetupPlaceholder(txtAddress, "Enter address")
        SetupPlaceholder(txtSearchOwner, "Search by owner name...")
        SetupPlaceholder(txtSearchOwnerApp, "Search by owner name...")
        SetupPlaceholder(txtSearchPet, "Search by pet name...")
        SetupPlaceholder(txtCashAmt, "Enter payment amount...")
        SetupPlaceholder(txtCurrentOwner, "Enter owner's name")
        SetupPlaceholder(txtAppOwner, "Enter owner name to verify")
    End Sub
    Private Sub SetupPlaceholder(textBox As TextBox, placeholderText As String)
        textBox.Text = placeholderText
        textBox.ForeColor = Color.Gray
        textBox.Tag = placeholderText ' Store placeholder text in Tag property

        ' Add event handlers
        AddHandler textBox.Enter, AddressOf TextBox_Enter
        AddHandler textBox.Leave, AddressOf TextBox_Leave
    End Sub

    Private Sub TextBox_Enter(sender As Object, e As EventArgs)
        Dim txt As TextBox = DirectCast(sender, TextBox)
        If txt.Text = txt.Tag.ToString() Then
            txt.Text = ""
            txt.ForeColor = Color.Black
        End If
    End Sub

    Private Sub TextBox_Leave(sender As Object, e As EventArgs)
        Dim txt As TextBox = DirectCast(sender, TextBox)
        If String.IsNullOrWhiteSpace(txt.Text) Then
            txt.Text = txt.Tag.ToString()
            txt.ForeColor = Color.Gray
        End If
    End Sub


    ' ========== LOAD DATA METHODS ==========
    Private Sub loadAllData()
        loadOwner()
        loadPet()
        loadAppointment()
        loadPayments()
    End Sub

    Public Sub loadOwner()
        'loads owner database file contents into owner table
        Dim loadOwnerTbl As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_owners.txt")
        Dim ownerInfos As String() = readFromFile(loadOwnerTbl)
        For Each line As String In ownerInfos
            Dim fields As String() = line.Split("|"c)
            If fields.Length >= 5 Then
                tblOwners.Rows.Add(fields(0), fields(1), fields(2), fields(3), fields(4))
            End If
        Next
    End Sub

    Public Sub loadPet()
        'loads pet database file contents into pet table
        Dim loadPetTbl As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_Pets.txt")
        Dim petInfos As String() = readFromFile(loadPetTbl)
        For Each line As String In petInfos
            Dim fields As String() = line.Split("|"c)
            If fields.Length >= 6 Then
                tblPets.Rows.Add(fields(0), fields(1), fields(5), fields(6), fields(7), fields(8))
            End If
        Next
    End Sub

    Public Sub loadAppointment()
        'loads appointment database file contents into appointment table
        Dim loadAppTbl As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_appointments.txt")
        Dim appointmentInfos As String() = readFromFile(loadAppTbl)
        For Each line As String In appointmentInfos
            Dim fields As String() = line.Split("|"c)
            If fields.Length >= 5 Then
                'determine status based on appointment date
                Dim status As String = "Upcoming"
                Dim appDate As Date
                If Date.TryParse(fields(0), appDate) Then
                    If appDate.Date < DateTime.Now.Date Then
                        status = "Completed"
                    ElseIf appDate.Date = DateTime.Now.Date Then
                        status = "Today"
                    End If
                End If

                ' displays in the appointments tab
                tblAppointments.Rows.Add(fields(0), fields(1), fields(2), fields(3), fields(4), status)
            End If
        Next
    End Sub

    Public Sub loadPayments()
        'loads payment database file contents into payment table
        Dim loadPaymentTbl As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_appointments.txt")
        Dim paymentInfos As String() = readFromFile(loadPaymentTbl)
        For Each line As String In paymentInfos
            Dim fields As String() = line.Split("|"c)
            If fields.Length >= 5 Then
                'determine status for payments table
                Dim status As String = "Upcoming"
                Dim appDate As Date
                If Date.TryParse(fields(0), appDate) Then
                    If appDate.Date < DateTime.Now.Date Then
                        status = "Completed"
                    ElseIf appDate.Date = DateTime.Now.Date Then
                        status = "Today"
                    End If
                End If

                'displays in the payment tab
                tblPayments.Rows.Add(fields(0), fields(1), fields(2), fields(3), fields(4), status)
            End If
        Next
    End Sub

    ' ========== ID GENERATION METHODS ==========
    Private Sub generateNextIDs()
        'calls both owner ID and pet ID generation methods to set next available IDs
        generateNextOwnerID()
        generateNextPetID()
    End Sub

    Private Sub generateNextPetID()
        'auto-generates the next available pet ID based on existing IDs in the pet table
        Dim nextID As Integer = 0
        For Each row As DataGridViewRow In tblPets.Rows
            If row.Cells(0).Value IsNot Nothing AndAlso IsNumeric(row.Cells(0).Value) Then
                Dim currentID As Integer = CInt(row.Cells(0).Value)
                If currentID >= nextID Then nextID = currentID + 1
            End If
        Next
        txtPetID.Text = nextID.ToString()
    End Sub

    Private Sub generateNextOwnerID()
        'auto-generates the next available owner ID based on existing IDs in the owner table
        Dim nextID As Integer = 0
        For Each row As DataGridViewRow In tblOwners.Rows
            If row.Cells(0).Value IsNot Nothing AndAlso IsNumeric(row.Cells(0).Value) Then
                Dim currentID As Integer = CInt(row.Cells(0).Value)
                If currentID >= nextID Then nextID = currentID + 1
            End If
        Next
        txtOwnerID.Text = nextID.ToString()
    End Sub

    ' ========== SEARCH METHODS ==========
    Private Function searchInTable(table As DataGridView, searchValue As String, cellIndex As Integer) As Boolean
        'clear any existing selection first
        table.ClearSelection()

        Dim foundAny As Boolean = False

        For i As Integer = 0 To table.Rows.Count - 1
            If table.Rows(i).Cells(cellIndex).Value IsNot Nothing Then
                Dim cellValue As String = table.Rows(i).Cells(cellIndex).Value.ToString()

                'check if cell value contains the search text (case-insensitive)
                If cellValue.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    'highlight the matching row
                    table.Rows(i).Selected = True
                    table.CurrentCell = table.Rows(i).Cells(cellIndex)
                    foundAny = True
                End If
            End If
        Next

        Return foundAny
    End Function

    Private Sub filterTableBySearch(table As DataGridView, searchValue As String, cellIndex As Integer)
        'show all rows first
        For Each row As DataGridViewRow In table.Rows
            'skip if row is not a data row
            If Not row.IsNewRow Then
                row.Visible = True
            End If
        Next

        'if search is empty, show all rows and exit
        If String.IsNullOrWhiteSpace(searchValue) Then
            Return
        End If

        'hide rows that don't match the search criteria
        For Each row As DataGridViewRow In table.Rows
            'skip new row (the empty row at the bottom for adding new data)
            If row.IsNewRow Then
                Continue For
            End If

            'check if cell index is valid and cell exists
            If cellIndex >= 0 AndAlso cellIndex < row.Cells.Count Then
                If row.Cells(cellIndex).Value IsNot Nothing Then
                    Dim cellValue As String = row.Cells(cellIndex).Value.ToString()
                    'show row if it contains the search text (case-insensitive)
                    row.Visible = cellValue.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0
                Else
                    'hide rows with null values in the search column
                    row.Visible = False
                End If
            Else
                'hide rows if cell index is invalid
                row.Visible = False
            End If
        Next
    End Sub

    Private Sub btnSearchOwner_Click(sender As Object, e As EventArgs) Handles btnSearchOwner.Click
        If String.IsNullOrWhiteSpace(txtSearchOwner.Text) Then
            'if search is empty, show all owners
            For Each row As DataGridViewRow In tblOwners.Rows
                If Not row.IsNewRow Then
                    row.Visible = True
                End If
            Next
            MessageBox.Show("Showing all owners.", "Search Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        'filter the owners table to show only matching names
        filterTableBySearch(tblOwners, txtSearchOwner.Text, 1) ' Column 1 = Owner Name

        'count visible rows (excluding new row)
        Dim visibleCount As Integer = 0
        For Each row As DataGridViewRow In tblOwners.Rows
            If Not row.IsNewRow AndAlso row.Visible Then
                visibleCount += 1
            End If
        Next

        If visibleCount > 0 Then
            'highlight the first matching row
            searchInTable(tblOwners, txtSearchOwner.Text, 1)
            MessageBox.Show($"Found {visibleCount} owner(s) matching '{txtSearchOwner.Text}'.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show($"No owners found matching '{txtSearchOwner.Text}'.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub btnSearchPet_Click(sender As Object, e As EventArgs) Handles btnSearchPet.Click
        If String.IsNullOrWhiteSpace(txtSearchPet.Text) Then
            'if search is empty, show all pets
            For Each row As DataGridViewRow In tblPets.Rows
                If Not row.IsNewRow Then
                    row.Visible = True
                End If
            Next
            MessageBox.Show("Showing all pets.", "Search Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        'filter the pets table to show only matching names
        filterTableBySearch(tblPets, txtSearchPet.Text, 1) ' Column 1 = Pet Name

        'count visible rows (excluding new row)
        Dim visibleCount As Integer = 0
        For Each row As DataGridViewRow In tblPets.Rows
            If Not row.IsNewRow AndAlso row.Visible Then
                visibleCount += 1
            End If
        Next

        If visibleCount > 0 Then
            'highlight the first matching row
            searchInTable(tblPets, txtSearchPet.Text, 1)
            MessageBox.Show($"Found {visibleCount} pet(s) matching '{txtSearchPet.Text}'.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show($"No pets found matching '{txtSearchPet.Text}'.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub btnGetAppointment_Click(sender As Object, e As EventArgs) Handles btnGetAppointment.Click
        If String.IsNullOrWhiteSpace(txtSearchOwnerApp.Text) Then
            'if search is empty, show all appointments
            For Each row As DataGridViewRow In tblAppointments.Rows
                If Not row.IsNewRow Then
                    row.Visible = True
                End If
            Next
            MessageBox.Show("Showing all appointments.", "Search Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        'filter the appointments table to show only matching owner names
        filterTableBySearch(tblAppointments, txtSearchOwnerApp.Text, 1) ' Column 1 = Owner Name

        'count visible rows (excluding new row)
        Dim visibleCount As Integer = 0
        For Each row As DataGridViewRow In tblAppointments.Rows
            If Not row.IsNewRow AndAlso row.Visible Then
                visibleCount += 1
            End If
        Next

        If visibleCount > 0 Then
            'highlight the first matching row
            searchInTable(tblAppointments, txtSearchOwnerApp.Text, 1)
            MessageBox.Show($"Found {visibleCount} appointment(s) for owner '{txtSearchOwnerApp.Text}'.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show($"No appointments found for owner '{txtSearchOwnerApp.Text}'.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'clear search filters when text is cleared (for owner tab)
    Private Sub txtSearchOwner_TextChanged(sender As Object, e As EventArgs) Handles txtSearchOwner.TextChanged
        If String.IsNullOrWhiteSpace(txtSearchOwner.Text) Then
            For Each row As DataGridViewRow In tblOwners.Rows
                If Not row.IsNewRow Then
                    row.Visible = True
                End If
            Next
            tblOwners.ClearSelection()
        End If
    End Sub

    'clear search filters when text is cleared (for pet tab)
    Private Sub txtSearchPet_TextChanged(sender As Object, e As EventArgs) Handles txtSearchPet.TextChanged
        If String.IsNullOrWhiteSpace(txtSearchPet.Text) Then
            For Each row As DataGridViewRow In tblPets.Rows
                If Not row.IsNewRow Then
                    row.Visible = True
                End If
            Next
            tblPets.ClearSelection()
        End If
    End Sub

    'clear search filters when text is cleared (for appointment tab)
    Private Sub txtSearchOwnerApp_TextChanged(sender As Object, e As EventArgs) Handles txtSearchOwnerApp.TextChanged
        If String.IsNullOrWhiteSpace(txtSearchOwnerApp.Text) Then
            For Each row As DataGridViewRow In tblAppointments.Rows
                If Not row.IsNewRow Then
                    row.Visible = True
                End If
            Next
            tblAppointments.ClearSelection()
        End If
    End Sub

    ' ========== VALIDATION METHODS ==========
    Private Function validateOwnerFields() As Boolean
        'called to validate if all owner input fields are filled
        If String.IsNullOrEmpty(txtOwnerName.Text) OrElse cmbSex.SelectedIndex = -1 OrElse
           String.IsNullOrEmpty(txtAddress.Text) OrElse numOwnerAge.Value <= 0 Then
            MessageBox.Show("Please fill in all owner fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function

    Private Function validatePetFields() As Boolean
        'called to validate if all pet input fields are filled
        If String.IsNullOrEmpty(txtPetName.Text) OrElse String.IsNullOrEmpty(txtWeight.Text) OrElse
           cmbType.SelectedIndex = -1 OrElse cmbVaccine.SelectedIndex = -1 OrElse
           String.IsNullOrEmpty(txtCurrentOwner.Text) Then
            MessageBox.Show("Please fill in all pet fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function

    Private Function validateAppointmentFields() As Boolean
        'called to validate if all appointment input fields are filled
        If cmbOwnersPets.SelectedIndex = -1 OrElse cmbProcedure.SelectedIndex = -1 Then
            MessageBox.Show("Please fill in all appointment fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function

    Private Function validateIDFormat(id As String) As Boolean
        'called to validate ID format using regex (only allows numbers, +, -, (, ), and spaces)
        Dim regex1 As New Regex("^[0-9+\-()\s]+$")
        Return regex1.IsMatch(id)
    End Function

    Private Function checkDuplicateID(table As DataGridView, id As String, cellIndex As Integer) As Boolean
        'checks for duplicate IDs in specified table and cell index
        For i As Integer = 0 To table.Rows.Count - 1
            If table.Rows(i).Cells(cellIndex).Value IsNot Nothing AndAlso
               table.Rows(i).Cells(cellIndex).Value.ToString() = id Then
                Return True
            End If
        Next
        Return False
    End Function

    ' ========== OWNER MANAGEMENT ==========
    Private Sub btnAddRecOwner_Click(sender As Object, e As EventArgs) Handles btnAddRecOwner.Click
        'checks if fields are filled
        If Not validateOwnerFields() Then Exit Sub

        'validates ID format (checks for invalid characters[only numbers allowed])
        If Not validateIDFormat(txtOwnerID.Text) Then
            MessageBox.Show("Invalid ID format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        'checks for duplicate ID
        If checkDuplicateID(tblOwners, txtOwnerID.Text, 0) Then
            MessageBox.Show("A pet owner already has that ID.", "Duplicate ID Detected", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        'gets the user input and saves it to ownerDetails string and prepares file path
        Dim ownerDetails As String = $"{txtOwnerID.Text}|{txtOwnerName.Text}|{numOwnerAge.Value}|{cmbSex.Text}|{txtAddress.Text}"
        Dim projectFilePath As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_owners.txt")

        'saves ownerDetails to owners database file
        saveToFile(projectFilePath, ownerDetails)

        'refreshes owner table and clears fields
        refreshTable(tblOwners, AddressOf loadOwner)
        clearOwnerFields()

        'confirmation message
        MessageBox.Show("Added owner.", "Added", MessageBoxButtons.OK, MessageBoxIcon.Information)

        'generates next owner ID and displays it
        generateNextOwnerID()
    End Sub

    Private Sub btnClearRecOwner_Click(sender As Object, e As EventArgs) Handles btnClearRecOwner.Click
        'clears owner input fields
        clearOwnerFields()
        MessageBox.Show("Fields Cleared.", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information)
        loadPlaceholders()
    End Sub

    ' ========== PET MANAGEMENT ==========
    Private Sub btnAddRecPet_Click(sender As Object, e As EventArgs) Handles btnAddRecPet.Click
        'checks if fields are filled
        If Not validatePetFields() Then Exit Sub

        'validates ID format (checks for invalid characters[only numbers allowed])
        If Not validateIDFormat(txtPetID.Text) Then
            MessageBox.Show("Invalid ID format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        'checks for duplicate pet ID
        If checkDuplicateID(tblPets, txtPetID.Text, 0) Then
            MessageBox.Show("A pet already has that ID.", "Duplicate Pet ID Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        'checks if owner exists
        If Not checkOwnerExists(txtCurrentOwner.Text) Then
            MessageBox.Show("Owner not found in the system. Double check or register first.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        'gets the user input and saves it to petDetails string and prepares file path
        Dim nextVisitScheduleString As String = "None"
        Dim petDetails As String = $"{txtPetID.Text}|{txtPetName.Text}|{numPetAge.Value}|{dtpBirthdate.Text}|{txtWeight.Text}|{cmbType.SelectedItem}|{cmbVaccine.SelectedItem}|{nextVisitScheduleString}|{txtCurrentOwner.Text}"
        Dim petFile As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_Pets.txt")

        'saves petDetails to pets database file
        saveToFile(petFile, petDetails)

        'refreshes pet table and clears fields
        refreshTable(tblPets, AddressOf loadPet)
        clearPetFields()

        'confirmation message
        MessageBox.Show("Added pet and assigned to owner.", "Added", MessageBoxButtons.OK, MessageBoxIcon.Information)

        'sets the next pet ID and displays it
        generateNextPetID()
    End Sub

    Private Sub btnClearRecPet_Click(sender As Object, e As EventArgs) Handles btnClearRecPet.Click
        'clears pet input fields
        clearPetFields()
        MessageBox.Show("Fields Cleared.", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information)
        loadPlaceholders()
    End Sub

    Private Function checkOwnerExists(ownerName As String) As Boolean
        'checks if owner exists in owner table
        For i As Integer = 0 To tblOwners.Rows.Count - 1
            If tblOwners.Rows(i).Cells(1).Value IsNot Nothing AndAlso
               tblOwners.Rows(i).Cells(1).Value.ToString() = ownerName Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub updateVaccineStatusAndNextVisit()
        'updates pet vaccine status and next visit when appointment date has passed
        Dim petsFilePath As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_Pets.txt")
        Dim appointmentsFilePath As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_appointments.txt")

        'prevents error if file is delete or not existing
        If Not IO.File.Exists(petsFilePath) OrElse Not IO.File.Exists(appointmentsFilePath) Then
            Return
        End If

        'read all pets and appointments
        Dim allPets As List(Of String) = IO.File.ReadAllLines(petsFilePath).ToList()
        Dim allAppointments As String() = IO.File.ReadAllLines(appointmentsFilePath)
        Dim updatedPets As New List(Of String)
        Dim updatedCount As Integer = 0

        'check each pet for passed appointments
        For Each petLine As String In allPets
            Dim fields As String() = petLine.Split("|"c)
            If fields.Length >= 9 Then
                Dim petName As String = fields(1)
                Dim ownerName As String = fields(8)
                Dim currentVaccineStatus As String = fields(6)
                Dim currentNextVisit As String = fields(7)
                Dim petUpdated As Boolean = False

                'check if this pet has any appointment that has passed
                If hasPassedAppointment(allAppointments, ownerName, petName) Then
                    'update next visit to "None"
                    fields(7) = "None"
                    petUpdated = True
                End If

                'check if vaccine status needs updating (only if "Incomplete")
                If currentVaccineStatus = "Incomplete" Then
                    'check if this pet has a vaccine appointment that has passed
                    If hasCompletedVaccineAppointment(allAppointments, ownerName, petName) Then
                        ' Update vaccine status to "Complete"
                        fields(6) = "Complete"
                        petUpdated = True
                    End If
                End If

                'count if any updates were made
                If petUpdated Then
                    updatedCount += 1
                End If

                'rebuild the pet line with updated fields
                updatedPets.Add(String.Join("|", fields))
            Else
                updatedPets.Add(petLine)
            End If
        Next

        'save updated pets back to file if changes were made
        If updatedCount > 0 Then
            IO.File.WriteAllLines(petsFilePath, updatedPets)
            'refresh the pets table to show updated data
            refreshTable(tblPets, AddressOf loadPet)
        End If
    End Sub

    Private Function hasPassedAppointment(allAppointments As String(), ownerName As String, petName As String) As Boolean
        'checks if a pet has any appointment that has passed
        For Each appointmentLine As String In allAppointments
            Dim fields As String() = appointmentLine.Split("|"c)
            If fields.Length >= 3 Then
                Dim appOwner As String = fields(1)
                Dim appPet As String = fields(2)
                Dim appDateStr As String = fields(0)

                'check if this appointment matches the pet and owner
                If appOwner.Trim() = ownerName.Trim() AndAlso appPet.Trim() = petName.Trim() Then
                    'parse the appointment date
                    Dim appDate As Date
                    If Date.TryParse(appDateStr, appDate) Then
                        'check if appointment date is today or in the past
                        If appDate.Date <= DateTime.Now.Date Then
                            Return True
                        End If
                    End If
                End If
            End If
        Next

        Return False
    End Function

    Private Function hasCompletedVaccineAppointment(allAppointments As String(), ownerName As String, petName As String) As Boolean
        'checks if a pet has a vaccine appointment that has passed
        For Each appointmentLine As String In allAppointments
            Dim fields As String() = appointmentLine.Split("|"c)
            If fields.Length >= 4 Then
                Dim appOwner As String = fields(1)
                Dim appPet As String = fields(2)
                Dim procedure As String = fields(3)
                Dim appDateStr As String = fields(0)

                'check if this appointment matches the pet and owner, and is a vaccine procedure
                If appOwner.Trim() = ownerName.Trim() AndAlso appPet.Trim() = petName.Trim() AndAlso
               (procedure.Trim() = "Vaccine" OrElse procedure.Trim() = "Vaccine-Checkup") Then

                    'parse the appointment date
                    Dim appDate As Date
                    If Date.TryParse(appDateStr, appDate) Then
                        'check if appointment date is today or in the past
                        If appDate.Date <= DateTime.Now.Date Then
                            Return True
                        End If
                    End If
                End If
            End If
        Next

        Return False
    End Function

    ' ========== APPOINTMENT MANAGEMENT ==========
    Private Sub btnVerify_Click(sender As Object, e As EventArgs) Handles btnVerify.Click

        'checks if owner exists in owner database to prevent invalid owner entries
        If Not searchInTable(tblOwners, txtAppOwner.Text, 1) Then
            MessageBox.Show("Owner not found", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            clearAppointmentFields()
            Exit Sub
        End If

        'if owner exists, enable appointment fields and load owner's pets
        MessageBox.Show("Owner found, appointment details now loading.", "Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
        enableAppointmentUI(True)
        loadOwnerPets(txtAppOwner.Text)
    End Sub

    Private Sub loadOwnerPets(ownerName As String)
        'clears previous items if there are any
        cmbOwnersPets.Items.Clear()

        'checks pet table for pets that belong to the verified owner and adds them to the combo box
        For Each petRow As DataGridViewRow In tblPets.Rows
            If petRow.Cells(5).Value IsNot Nothing AndAlso petRow.Cells(5).Value.ToString() = ownerName Then
                cmbOwnersPets.Items.Add(petRow.Cells(1).Value.ToString())
            End If
        Next

        'selects the first pet in the list if there are any pets, otherwise shows message
        If cmbOwnersPets.Items.Count > 0 Then
            cmbOwnersPets.SelectedIndex = 0
        Else
            MessageBox.Show("This owner has no registered pets.", "No Pets Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As EventArgs) Handles btnCreate.Click
        'checks if appointment fields are filled
        If Not validateAppointmentFields() Then Exit Sub

        'validates appointment date (no past dates and no Sundays)
        If Not validateAppointmentDate() Then Exit Sub

        'avoids duplicate appointments on the same date
        If checkDuplicateAppointment() Then Exit Sub

        'calculates total price based on pet type and procedure
        checkPrice()

        'gets appointment details and saves them into variables
        Dim appDate As String = dtpDateApp.Value.ToString("MM/dd/yyyy")
        Dim appOwner As String = txtAppOwner.Text
        Dim appPet As String = cmbOwnersPets.SelectedItem.ToString()
        Dim appProcedure As String = cmbProcedure.SelectedItem.ToString()
        Dim appTotal As String = total.ToString()

        'stores appointment details in pending variable for payment processing
        pendingAppointmentDetails = $"{appDate}|{appOwner}|{appPet}|{appProcedure}|{appTotal}"

        'disables appointment fields and buttons to prevent multiple bookings
        enableAppointmentUI(False)
        enableAppointmentButtons(False)

        'confirmation message and enables payment fields
        MessageBox.Show("Appointment details saved. Proceed to Payment to complete Appointment booking.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        txtCashAmt.ReadOnly = False
        btnPay.Enabled = True
    End Sub

    Private Sub btnCancelApp_Click(sender As Object, e As EventArgs) Handles btnCancelApp.Click
        'deletes pending appointment details and resets UI
        clearAppointmentFields()
        clearPaymentFields()
        enableAppointmentUI(False)
        enableAppointmentButtons(True)
        txtCashAmt.ReadOnly = True
        btnPay.Enabled = False
        txtAppOwner.ReadOnly = False
        loadPlaceholders()
    End Sub

    Private Function validateAppointmentDate() As Boolean
        'validates appointment date (no past dates and no Sundays)
        If dtpDateApp.Value.Date < DateTime.Now.Date Then
            MessageBox.Show("You cannot select a past date for the appointment.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            dtpDateApp.Value = DateTime.Now
            Return False
        End If

        'validates if selected date is a Sunday (clinic is closed on Sundays)
        If dtpDateApp.Value.DayOfWeek = DayOfWeek.Sunday Then
            MessageBox.Show("The clinic is closed on Sundays. Please select another date.", "Clinic Closed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            dtpDateApp.Value = DateTime.Now
            Return False
        End If

        Return True
    End Function

    Private Function checkDuplicateAppointment() As Boolean
        'checks for existing appointments on the selected date to avoid duplicates
        Dim selectedDate As Date = dtpDateApp.Value.Date
        For Each row As DataGridViewRow In tblAppointments.Rows
            If row.Cells(0).Value IsNot Nothing Then
                Dim existingDateString As String = row.Cells(0).Value.ToString()
                Dim existingDate As Date
                If Date.TryParse(existingDateString, existingDate) AndAlso existingDate.Date = selectedDate Then
                    MessageBox.Show("There is already an appointment scheduled on " & selectedDate.ToString("MM/dd/yyyy") & ". Only one appointment per day is allowed.", "Date Already Booked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    Private Sub saveAppointment(appDate As String, appOwner As String, appPet As String, appProcedure As String, appTotal As String)
        'saves appointment details to appointment database file
        Dim details As String = $"{appDate}|{appOwner}|{appPet}|{appProcedure}|{appTotal}"
        Dim filePath As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_appointments.txt")
        saveToFile(filePath, details)
    End Sub

    ' ========== PAYMENT SYSTEM ==========
    Public Sub checkPrice()
        'creates local variables to hold the specific pet and its type(Canine, Feline, Reptile)
        Dim petType As String = ""
        Dim selectedPet As String = cmbOwnersPets.SelectedItem?.ToString()

        'finds the selected pet in the pet table to get its type
        If Not String.IsNullOrEmpty(selectedPet) Then
            For Each row As DataGridViewRow In tblPets.Rows
                If row.Cells(1).Value IsNot Nothing AndAlso row.Cells(1).Value.ToString() = selectedPet Then
                    petType = row.Cells(2).Value?.ToString()
                    Exit For
                End If
            Next
        End If

        'gets the selected procedure (Checkup, Vaccine, Vaccine-Checkup) 
        Dim procedure As String = cmbProcedure.SelectedItem?.ToString()

        'calculates the total price based on pet type and procedure
        total = calculatePrice(petType, procedure)

        'displays the appointment details including total price in the payment details list box
        Dim appDetailsString As New StringBuilder()
        appDetailsString.AppendLine("Name: " + txtAppOwner.Text)
        appDetailsString.AppendLine("Pet: " + cmbOwnersPets.Text)
        appDetailsString.AppendLine("Type: " + petType)
        appDetailsString.AppendLine("Appointment Date: " + dtpDateApp.Value.ToString("MM/dd/yyyy"))
        appDetailsString.AppendLine("Procedure: " + cmbProcedure.Text)
        appDetailsString.AppendLine("Total price: " + total.ToString)
        listAppDetails1.Text = appDetailsString.ToString()
    End Sub

    Private Sub btnPay_Click(sender As Object, e As EventArgs) Handles btnPay.Click
        If Val(txtCashAmt.Text) >= total Then
            'only save appointment to database AFTER successful payment
            If Not String.IsNullOrEmpty(pendingAppointmentDetails) Then
                Dim fields As String() = pendingAppointmentDetails.Split("|"c)
                If fields.Length >= 5 Then
                    'Save appointment to file
                    saveAppointment(fields(0), fields(1), fields(2), fields(3), fields(4))

                    'Update pet's visit schedule
                    updatePetVisitSchedule(fields(1), fields(2), fields(0))

                    'Refresh appointment table to show the new appointment
                    refreshTable(tblAppointments, AddressOf loadAppointment)

                    'Refresh payment table to show the new payment
                    refreshTable(tblPayments, AddressOf loadPayments)
                End If
            End If

            'payment confirmation message
            MessageBox.Show("Payment Successful! Appointment added. " + "Customer change: " + (Val(txtCashAmt.Text) - total).ToString, "Appointment Confirmed", MessageBoxButtons.OK, MessageBoxIcon.Information)

            'reset UI for the next appointment
            txtCashAmt.ReadOnly = True
            btnPay.Enabled = False
            clearPaymentFields()
            clearAppointmentFields()
            txtAppOwner.ReadOnly = False
            enableAppointmentButtons(True)

            'reset pending appointment
            pendingAppointmentDetails = ""
        Else
            'output error message for insufficient cash amount
            MessageBox.Show("Insufficient Cash Amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ' ========== VISIT SCHEDULE UPDATE ==========
    Private Sub updatePetVisitSchedule(ownerName As String, petName As String, visitDate As String)
        'gets the pet database file path for conditional update
        Dim petFilePath As String = IO.Path.Combine(Application.StartupPath, "..\..\Resources\Text Files\NomVC_Pets.txt")
        Dim allPets As List(Of String) = IO.File.ReadAllLines(petFilePath).ToList()
        Dim updatedPets As New List(Of String)

        'iterates through each pet record to find the matching owner and pet name to update the visit schedule
        For Each petLine As String In allPets
            Dim fields As String() = petLine.Split("|"c)
            If fields.Length >= 9 Then
                If fields(8).Trim() = ownerName.Trim() AndAlso fields(1).Trim() = petName.Trim() Then
                    fields(7) = visitDate
                End If
                updatedPets.Add(String.Join("|", fields))
            Else
                updatedPets.Add(petLine)
            End If
        Next

        'saves the updated pet records back to the pet database file and reloads the pet table
        IO.File.WriteAllLines(petFilePath, updatedPets)
        refreshTable(tblPets, AddressOf loadPet)
    End Sub

    ' ========== DISPLAY DETAILS METHODS ==========
    Private Sub tblOwners_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles tblOwners.CellClick
        'if a valid row is clicked, calls displayOwnerDetails to show owner details
        If e.RowIndex >= 0 AndAlso e.RowIndex < tblOwners.Rows.Count Then
            displayOwnerDetails(e.RowIndex)
        End If
    End Sub

    Private Sub displayOwnerDetails(rowIndex As Integer)
        'called to display owner details along with their pets in the owner details list box
        Dim row As DataGridViewRow = tblOwners.Rows(rowIndex)
        Dim ownerName As String = row.Cells(1).Value?.ToString()

        'check if ownerName is null or empty
        If String.IsNullOrEmpty(ownerName) Then
            listOwnerDetails.Text = "No owner selected or invalid row."
            Return
        End If

        'rich text box to hold owner details
        Dim details As New StringBuilder()
        details.AppendLine($"Name:  {If(row.Cells(1).Value?.ToString(), "N/A")} [{If(row.Cells(0).Value?.ToString(), "N/A")}]")
        details.AppendLine($"Age: {If(row.Cells(2).Value?.ToString(), "N/A")} year(s) old")
        details.AppendLine("Sex: " & If(row.Cells(3).Value?.ToString(), "N/A"))
        details.AppendLine("Address: " & If(row.Cells(4).Value?.ToString(), "N/A"))
        details.AppendLine("")
        details.AppendLine("List of Pets: ")

        'lists all pets that belong to the selected owner
        Dim petCount As Integer = 0
        For Each petRow As DataGridViewRow In tblPets.Rows
            ' Skip if petRow is the new row or if Cells(5) is null
            If Not petRow.IsNewRow AndAlso petRow.Cells(5).Value IsNot Nothing Then
                Dim petOwnerName As String = petRow.Cells(5).Value.ToString()
                If Not String.IsNullOrEmpty(petOwnerName) AndAlso petOwnerName.Trim() = ownerName.Trim() Then
                    petCount += 1
                    details.AppendLine($"{petCount}. {If(petRow.Cells(1).Value?.ToString(), "N/A")}")
                    details.AppendLine($"   • Type: {If(petRow.Cells(2).Value?.ToString(), "N/A")}")
                    details.AppendLine($"   • Vaccine: {If(petRow.Cells(3).Value?.ToString(), "N/A")}")
                    details.AppendLine($"   • Next Visit: {If(petRow.Cells(4).Value?.ToString(), "None")}")
                    details.AppendLine("")
                End If
            End If
        Next

        'displays if owner has no pets
        If petCount = 0 Then details.AppendLine("No pets registered")
        listOwnerDetails.Text = details.ToString()
    End Sub

    Private Sub tblPets_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles tblPets.CellClick
        'if a valid row is clicked, calls displayPetDetails to show pet details
        If e.RowIndex >= 0 AndAlso e.RowIndex < tblPets.Rows.Count Then
            displayPetDetails(e.RowIndex)
        End If
    End Sub

    Private Sub displayPetDetails(rowIndex As Integer)
        'called to display full pet details in the pet details list box
        Dim row As DataGridViewRow = tblPets.Rows(rowIndex)
        Dim petID As String = row.Cells(0).Value?.ToString()

        ' Check if petID is null or empty (empty row)
        If String.IsNullOrEmpty(petID) Then
            listPetDetails.Text = "No pet selected or invalid row."
            Return
        End If

        Dim petDetails As String() = getPetDetailsFromFile(petID)

        'rich text box to hold pet details
        If petDetails IsNot Nothing Then
            Dim details As New StringBuilder()
            details.AppendLine($"Name: {If(petDetails(1), "N/A")} [{If(petDetails(0), "N/A")}]")
            details.AppendLine("")
            details.AppendLine($"Age: {If(petDetails(2), "N/A")} year(s) old")
            details.AppendLine("")
            details.AppendLine($"Birthday: {If(petDetails(3), "N/A")}")
            details.AppendLine("")
            details.AppendLine($"Weight: {If(petDetails(4), "N/A")} kg")
            details.AppendLine("")
            details.AppendLine($"Type: {If(petDetails(5), "N/A")}")
            details.AppendLine("")
            details.AppendLine($"Vaccine Status: {If(petDetails(6), "N/A")}")
            details.AppendLine("")
            details.AppendLine($"Next Visit: {If(petDetails(7), "None")}")
            details.AppendLine("")
            details.AppendLine($"Owner: {If(petDetails(8), "N/A")}")

            listPetDetails.Text = details.ToString()
        Else
            listPetDetails.Text = "Pet details not found in database."
        End If
    End Sub

    Private Sub tblAppointments_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles tblAppointments.CellClick
        'if a valid row is clicked, calls displayAppointmentDetails to show appointment details
        If e.RowIndex >= 0 AndAlso e.RowIndex < tblAppointments.Rows.Count Then
            displayAppointmentDetails(e.RowIndex)
        End If
    End Sub

    Private Sub displayAppointmentDetails(rowIndex As Integer)
        'called to display appointment details including calculated total price in the appointment details list box
        Dim row As DataGridViewRow = tblAppointments.Rows(rowIndex)
        Dim appointmentDate As String = row.Cells(0).Value?.ToString()

        'check if appointmentDate is null or empty (empty row)
        If String.IsNullOrEmpty(appointmentDate) Then
            listAppDetails.Text = "No appointment selected or invalid row."
            Return
        End If

        'safely get values from cells
        Dim ownerName As String = If(row.Cells(1).Value?.ToString(), "N/A")
        Dim petName As String = If(row.Cells(2).Value?.ToString(), "N/A")
        Dim procedure As String = If(row.Cells(3).Value?.ToString(), "N/A")
        Dim totalPrice As String = If(row.Cells(4).Value?.ToString(), "N/A")
        Dim status As String = If(row.Cells(5).Value?.ToString(), "N/A")

        Dim petType As String = getPetType(ownerName, petName)

        'rich text box to hold appointment details
        Dim details As New StringBuilder()
        details.AppendLine($"Owner Name: {ownerName}")
        details.AppendLine("")
        details.AppendLine($"Pet: {petName}")
        details.AppendLine("")
        details.AppendLine($"Pet Type: {If(petType, "N/A")}")
        details.AppendLine("")
        details.AppendLine($"Procedure: {procedure}")
        details.AppendLine("")
        details.AppendLine($"Appointment Date: {appointmentDate}")
        details.AppendLine("")
        details.AppendLine($"Status: {status}")
        details.AppendLine("")
        details.AppendLine($"Total Price: ₱{totalPrice}")

        listAppDetails.Text = details.ToString()
    End Sub


    ' ========== UTILITY METHODS ==========
    Private Sub refreshTable(table As DataGridView, loadMethod As Action)
        table.Rows.Clear()
        loadMethod()
    End Sub

    Private Sub enableAppointmentUI(enabled As Boolean)
        cmbOwnersPets.Enabled = enabled
        cmbProcedure.Enabled = enabled
        dtpDateApp.Enabled = enabled
    End Sub

    Private Sub enableAppointmentButtons(enabled As Boolean)
        btnVerify.Enabled = enabled
        btnCreate.Enabled = enabled
    End Sub

    ' ========== CLEAR METHODS ==========
    Public Sub clearOwnerFields()
        txtOwnerName.Clear()
        numOwnerAge.Value = 0
        cmbSex.SelectedIndex = -1
        cmbSex.Text = ""
        txtAddress.Clear()
    End Sub

    Private Sub clearPetFields()
        txtPetName.Clear()
        txtWeight.Clear()
        txtCurrentOwner.Clear()
        numPetAge.Value = numPetAge.Minimum
        dtpBirthdate.Value = DateTime.Now
        cmbType.SelectedIndex = -1
        cmbVaccine.SelectedIndex = -1
        cmbType.Text = ""
        cmbVaccine.Text = ""
    End Sub

    Private Sub clearAppointmentFields()
        txtAppOwner.Clear()
        txtSearchOwnerApp.Clear()
        cmbOwnersPets.Items.Clear()
        cmbOwnersPets.SelectedIndex = -1
        cmbProcedure.SelectedIndex = -1
        dtpDateApp.Value = DateTime.Now
        cmbOwnersPets.Text = ""
    End Sub

    Private Sub clearPaymentFields()
        listAppDetails1.Clear()
        Dim appDetailsString As New StringBuilder()
        appDetailsString.AppendLine("Name: ")
        appDetailsString.AppendLine("Pet: ")
        appDetailsString.AppendLine("Type: ")
        appDetailsString.AppendLine("Appointment Date: ")
        appDetailsString.AppendLine("Procedure: ")
        appDetailsString.AppendLine("Total price: ")
        listAppDetails1.Text = appDetailsString.ToString()
        txtCashAmt.Clear()
    End Sub

    Private Sub lblLogout_Click(sender As Object, e As EventArgs) Handles lblLogout.Click
        Me.Close()
        Login.Show()
        Login.txtPass.Clear()
        Login.txtUser.Clear()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
    End Sub
End Class