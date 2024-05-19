import { Button, Container, TextField, Typography } from "@mui/material";
import { FC, useEffect } from "react";
import useLoginHook from "../../../Features/User/LoginHook";
import useGetErrorMessageHook from "../../../Features/Error/getErrorMessageHook";

const Login: FC = () => {
  const {
    login,
    setLogin,
    password,
    setPassword,
    isLoading,
    isFetching,
    error,
    handleLogin,
  } = useLoginHook();

  const errorMessage = useGetErrorMessageHook(error);

  useEffect(() => console.log(error), [error]);

  return (
    <>
      <Container
        maxWidth="sm"
        style={{
          width: 400,
          height: "100vh",
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "center",
        }}
      >
        <Typography variant="h4" align="center" gutterBottom>
          Login
        </Typography>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleLogin();
          }}
          style={{ width: "100%", marginTop: "20px" }}
        >
          <TextField
            label="Username"
            variant="outlined"
            fullWidth
            margin="normal"
            value={login}
            onChange={(e) => setLogin(e.target.value)}
          />
          <TextField
            label="Password"
            variant="outlined"
            type="password"
            fullWidth
            margin="normal"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          {errorMessage && (
            <Typography
              variant="body2"
              color="error"
              align="center"
              gutterBottom
            >
              {errorMessage == "Forbid"
                ? "Incorect login or password"
                : errorMessage}
            </Typography>
          )}
          <Button
            type="submit"
            variant="contained"
            color="primary"
            fullWidth
            size="large"
            disabled={isLoading || isFetching}
            style={{ marginTop: "20px" }}
          >
            Login
          </Button>
        </form>
      </Container>
    </>
  );
};

export default Login;
