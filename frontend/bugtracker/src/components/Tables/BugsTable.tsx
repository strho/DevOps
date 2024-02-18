import { useMemo, useState } from 'react';
import {
  MRT_EditActionButtons as MRTEditActionButtons,
  MaterialReactTable,
  // createRow,
  type MRT_ColumnDef,
  type MRT_Row,
  type MRT_TableOptions,
  useMaterialReactTable,
} from 'material-react-table';
import {
  Box,
  Button,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  QueryClient,
  QueryClientProvider,
  useMutation,
  useQuery,
  useQueryClient,
} from '@tanstack/react-query';
import { Bug, BugService } from '../../services/bugsservice';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

const service = new BugService();

const BugsTable = () => {
  const [validationErrors, setValidationErrors] = useState<
    Record<string, string | undefined>
  >({});


  const columns = useMemo<MRT_ColumnDef<Bug>[]>(
    () => [
      {
        accessorKey: 'id',
        header: 'Id',
        enableEditing: false,
        size: 80,
      },
      {
        accessorKey: 'title',
        header: 'Title',
        muiEditTextFieldProps: {
          required: true,
          error: !!validationErrors?.title,
          helperText: validationErrors?.title,
          //remove any previous validation errors when bugs focuses on the input
          onFocus: () =>
            setValidationErrors({
              ...validationErrors,
              title: undefined,
            }),
          //optionally add validation checking for onBlur or onChange
        },
      },
      {
        accessorKey: 'description',
        header: 'Description',
        muiEditTextFieldProps: {
          type: 'description',
          required: true,
          error: !!validationErrors?.description,
          helperText: validationErrors?.description,
          //remove any previous validation errors when bug focuses on the input
          onFocus: () =>
            setValidationErrors({
              ...validationErrors,
              description: undefined,
            }),
        },
      },
      {
        accessorKey: 'status',
        header: 'Status',
        editVariant: 'select',
        editSelectOptions: service.getStatuses(),
        muiEditTextFieldProps: {
          select: true,
          error: !!validationErrors?.status,
          helperText: validationErrors?.status,
        },
      },
    ],
    [validationErrors],
  );

  //call CREATE hook
  const { mutateAsync: createBug, isPending: isCreatingBug } =
    useCreateBug();
  //call READ hook
  const {
    data: fetchedBugs = [],
    isError: isLoadingBugsError,
    isFetching: isFetchingBugs,
    isLoading: isLoadingBugs,
  } = useGetBugs();
  //call UPDATE hook
  const { mutateAsync: updateBug, isPending: isUpdatingBug } =
    useUpdateBug();
  //call DELETE hook
  const { mutateAsync: deleteBug, isPending: isDeletingBug } =
    useDeleteBug();

  //CREATE action
  const handleCreateBug: MRT_TableOptions<Bug>['onCreatingRowSave'] = async ({
    values,
    table,
  }) => {
    const newValidationErrors = validateBug(values);
    if (Object.values(newValidationErrors).some((error) => error)) {
      setValidationErrors(newValidationErrors);
      return;
    }
    setValidationErrors({});
    await createBug(values);
    table.setCreatingRow(null); //exit creating mode
  };

  //UPDATE action
  const handleSaveBug: MRT_TableOptions<Bug>['onEditingRowSave'] = async ({
    values,
    table,
  }) => {
    const newValidationErrors = validateBug(values);
    if (Object.values(newValidationErrors).some((error) => error)) {
      setValidationErrors(newValidationErrors);
      return;
    }
    setValidationErrors({});
    await updateBug(values);
    table.setEditingRow(null); //exit editing mode
  };

  //DELETE action
  const openDeleteConfirmModal = (row: MRT_Row<Bug>) => {
    if (window.confirm('Are you sure you want to delete this bug?')) {
      deleteBug(row.original.id);
    }
  };

  const table = useMaterialReactTable({
    columns,
    data: fetchedBugs,
    createDisplayMode: 'modal', //default ('row', and 'custom' are also available)
    editDisplayMode: 'modal', //default ('row', 'cell', 'table', and 'custom' are also available)
    enableEditing: true,
    enableHiding: false,
    enableDensityToggle: false,
    enableFullScreenToggle: false,
    getRowId: (row) => `${row.id}`,
    muiToolbarAlertBannerProps: isLoadingBugsError
      ? {
        color: 'error',
        children: 'Error loading data',
      }
      : undefined,
    muiTableContainerProps: {
      sx: {
        minHeight: '500px',
      },
    },
    onCreatingRowCancel: () => setValidationErrors({}),
    onCreatingRowSave: handleCreateBug,
    onEditingRowCancel: () => setValidationErrors({}),
    onEditingRowSave: handleSaveBug,
    //optionally customize modal content
    renderCreateRowDialogContent: ({ table, row, internalEditComponents }) => (
      <>
        <DialogTitle variant="h3">Create New Bug</DialogTitle>
        <DialogContent
          sx={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}
        >
          {internalEditComponents} {/* or render custom edit components here */}
        </DialogContent>
        <DialogActions>
          <MRTEditActionButtons variant="text" table={table} row={row} />
        </DialogActions>
      </>
    ),
    //optionally customize modal content
    renderEditRowDialogContent: ({ table, row, internalEditComponents }) => (
      <>
        <DialogTitle variant="h3">Edit Bug</DialogTitle>
        <DialogContent
          sx={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}
        >
          {internalEditComponents} {/* or render custom edit components here */}
        </DialogContent>
        <DialogActions>
          <MRTEditActionButtons variant="text" table={table} row={row} />
        </DialogActions>
      </>
    ),
    renderRowActions: ({ row, table }) => (
      <Box sx={{ display: 'flex', gap: '1rem' }}>
        <Tooltip title="Edit">
          <IconButton onClick={() => table.setEditingRow(row)}>
            <EditIcon />
          </IconButton>
        </Tooltip>
        <Tooltip title="Delete">
          <IconButton color="error" onClick={() => openDeleteConfirmModal(row)}>
            <DeleteIcon />
          </IconButton>
        </Tooltip>
      </Box>
    ),
    renderTopToolbarCustomActions: ({ table }) => (
      <Button
        variant="contained"
        onClick={() => {
          table.setCreatingRow(true); //simplest way to open the create row modal with no default values
          //or you can pass in a row object to set default values with the `createRow` helper function
          // table.setCreatingRow(
          //   createRow(table, {
          //     //optionally pass in default values for the new row, useful for nested data or other complex scenarios
          //   }),
          // );
        }}
      >
        Create New Bug
      </Button>
    ),
    state: {
      isLoading: isLoadingBugs,
      isSaving: isCreatingBug || isUpdatingBug || isDeletingBug,
      showAlertBanner: isLoadingBugsError,
      showProgressBars: isFetchingBugs,
    },
  });

  return <MaterialReactTable table={table} />;
};

//CREATE hook (post new bug to api)
function useCreateBug() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (bug: Bug) => {
      await service.createBug(bug);
    },
    //client side optimistic update
    onMutate: (newBugInfo: Bug) => {
      queryClient.setQueryData(
        ['bugs'],
        (prevBugs: any) =>
          [
            ...prevBugs,
            {
              ...newBugInfo,
              id: (Math.random() + 1).toString(36).substring(7),
            },
          ] as Bug[],
      );
    },
    onSettled: () => queryClient.invalidateQueries({ queryKey: ['bugs'] }),
  });
}

//READ hook (get bugs from api)
function useGetBugs() {
  return useQuery<Bug[]>({
    queryKey: ['bugs'],
    queryFn: async () => {
      return await service.getBugs();
    },
    refetchOnWindowFocus: false,
  });
}

//UPDATE hook (put bug in api)
function useUpdateBug() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (bug: Bug) => {
      await service.updateBug(bug);
    },
    //client side optimistic update
    onMutate: (newBugInfo: Bug) => {
      queryClient.setQueryData(['bugs'], (prevBugs: any) =>
        prevBugs?.map((prevBug: Bug) =>
          prevBug.id === newBugInfo.id ? newBugInfo : prevBug,
        ),
      );
    },
    onSettled: () => queryClient.invalidateQueries({ queryKey: ['bugs'] }),
  });
}

//DELETE hook (delete bug in api)
function useDeleteBug() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (bugId: number) => {
      await service.deleteBug(bugId);
    },
    //client side optimistic update
    onMutate: (bugId: number) => {
      queryClient.setQueryData(['bugs'], (prevBugs: any) =>
        prevBugs?.filter((bug: Bug) => bug.id !== bugId),
      );
    },
    onSettled: () => queryClient.invalidateQueries({ queryKey: ['bugs'] }),
  });
}

const queryClient = new QueryClient();

const ExampleWithProviders = () => (
  //Put this with your other react-query providers near root of your app
  <QueryClientProvider client={queryClient}>
    <div style={{ width: "100%" }}>
      <BugsTable />
    </div>
  </QueryClientProvider>
);

export default ExampleWithProviders;

const validateRequired = (value: string) => !!value.length;

function validateBug(bug: Bug) {
  return {
    name: !validateRequired(bug.title)
      ? 'Title is Required'
      : '',
  };
}